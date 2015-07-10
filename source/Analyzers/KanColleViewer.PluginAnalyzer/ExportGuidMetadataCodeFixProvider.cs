using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Grabacr07.KanColleViewer.PluginAnalyzer
{
	/// <summary>
	/// GUID の ExportMetadata 属性が定義されていない場合にコード修正を行う CodeFixProvider です。
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExportGuidMetadataCodeFixProvider)), Shared]
	public class ExportGuidMetadataCodeFixProvider : CodeFixProvider
	{
		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ExportGuidMetadataAnalyzer.DiagnosticId);

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public override sealed async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false) as CompilationUnitSyntax;

			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;
			var classDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

			var model = await context.Document.GetSemanticModelAsync();
			var compilation = model.Compilation;
			var classes = compilation.SyntaxTrees
				.Select(x => x.GetCompilationUnitRoot())
				.SelectMany(x => x.FindSyntax<ClassDeclarationSyntax>())
				.Where(x => x.IsPluginClass())
				.ToArray();
			var plugins = classes.Where(x => x.IsExportIPlugin()).ToArray();
			var features = classes.Except(plugins);

			var isExportIPlugin = plugins.Contains(classDeclaration);
			if (isExportIPlugin)
			{
				//IPluginの場合新GUIDを候補に
				var codeAction = CodeAction.Create(
					"Add New Guid",
					_ => AddGuidExportMetadata(context.Document, root, classDeclaration, Utility.NewGuidValue()),
					$"{ExportGuidMetadataAnalyzer.DiagnosticId} Add New Guid");
				context.RegisterCodeFix(codeAction, diagnostic);

				//他のIPluginで使われてないGUIDを持ってるPluginFeatureがいたらそのGUIDを候補に
				var candidates = features.Where(x => plugins.All(y => x.GetGuidMetadataValue() != y.GetGuidMetadataValue()));
				foreach (var candidate in candidates)
				{
					var additionalAction = CodeAction.Create(
						$"Add Guid of {candidate.Identifier}",
						_ => AddGuidExportMetadata(context.Document, root, classDeclaration, candidate.GetGuidMetadataValue()),
						$"{ExportGuidMetadataAnalyzer.DiagnosticId} Add Guid of {candidate.Identifier}");
					context.RegisterCodeFix(additionalAction, diagnostic);
				}
			}
			else if (plugins.Any())
			{
				//PluginFeatureの場合で、IPlugin実装が存在するならそのGUIDを候補に
				foreach (var plugin in plugins)
				{
					var additionalAction = CodeAction.Create(
						$"Add Guid of {plugin.Identifier}",
						_ => AddGuidExportMetadata(context.Document, root, classDeclaration, plugin.GetGuidMetadataValue()),
						$"{ExportGuidMetadataAnalyzer.DiagnosticId} Add Guid of {plugin.Identifier}");
					context.RegisterCodeFix(additionalAction, diagnostic);
				}

				//新GUIDも候補に
				var codeAction = CodeAction.Create(
					"Add New Guid",
					_ => AddGuidExportMetadata(context.Document, root, classDeclaration, Utility.NewGuidValue()),
					$"{ExportGuidMetadataAnalyzer.DiagnosticId} Add New Guid");
				context.RegisterCodeFix(codeAction, diagnostic);
			}
			else
			{
				//PluginFeatureの場合で、IPlugin実装が存在しないなら新GUIDを候補に
				var codeAction = CodeAction.Create(
					"Add New Guid",
					_ => AddGuidExportMetadata(context.Document, root, classDeclaration, Utility.NewGuidValue()),
					$"{ExportGuidMetadataAnalyzer.DiagnosticId} Add New Guid");
				context.RegisterCodeFix(codeAction, diagnostic);
			}
		}

		private static Task<Document> AddGuidExportMetadata(Document document, CompilationUnitSyntax root, ClassDeclarationSyntax classDeclaration, string guid)
		{
			var newSyntax = classDeclaration;
			var attributeList = SyntaxFactory.AttributeList(
				SyntaxFactory.SingletonSeparatedList(
					SyntaxFactory.Attribute(SyntaxFactory.ParseName("ExportMetadata"),
						SyntaxFactory.ParseAttributeArgumentList($"(\"Guid\", \"{guid}\")"))));
			newSyntax = newSyntax.AddAttributeLists(attributeList);
			var newRoot = root.ReplaceNode(classDeclaration, newSyntax);
			var newDocument = document.WithSyntaxRoot(newRoot);
			return Task.FromResult(newDocument);
		}
	}
}
