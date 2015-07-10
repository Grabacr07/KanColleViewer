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
	/// 不正な GUID の ExportMetadata が存在する場合にコード修正を行う CodeFixProvider です。
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InvalidGuidMetadataCodeFixProvider)), Shared]
	public class InvalidGuidMetadataCodeFixProvider : CodeFixProvider
	{
		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(InvalidGuidMetadataAnalyzer.DiagnosticId);

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
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
				.ToArray();
			var plugins = classes.Where(x => x.IsExportIPlugin()).ToArray();

			var guidValue = classDeclaration.GetGuidMetadataValue();

			// IPlugin で未定義の GUID が指定されていた場合、既存の IPlugin の GUID を全て候補に
			if (plugins.All(x => x.GetGuidMetadataValue() != guidValue))
			{
				foreach (var plugin in plugins)
				{
					var codeAction = CodeAction.Create(
						$"Use Guid of {plugin.Identifier}",
						_ => FixGuidExportMetadata(context.Document, root, classDeclaration, plugin.GetGuidMetadataValue()),
						$"{InvalidGuidMetadataAnalyzer.DiagnosticId} Use Guid of {plugin.Identifier}");
					context.RegisterCodeFix(codeAction, diagnostic);
				}
			}
			
			// GUID として解釈が指定されていた場合、新 GUID を候補に
			Guid _guid;
			if (!Guid.TryParse(guidValue, out _guid))
			{
				var codeAction = CodeAction.Create(
					"Use New Guid",
					_ => FixGuidExportMetadata(context.Document, root, classDeclaration, Utility.NewGuidValue()),
					$"{InvalidGuidMetadataAnalyzer.DiagnosticId} Use New Guid");
				context.RegisterCodeFix(codeAction, diagnostic);
			}
		}

		private static Task<Document> FixGuidExportMetadata(Document document, CompilationUnitSyntax root, ClassDeclarationSyntax classDeclaration, string guid)
		{
			var newSyntax = classDeclaration;
			
			newSyntax = newSyntax.ReplaceNode(
				newSyntax.GetAttributes("ExportMetadata").First(x => x.ArgumentList.Arguments[0].ToString() == "\"Guid\""),
				SyntaxFactory.Attribute(SyntaxFactory.ParseName("ExportMetadata"),
					SyntaxFactory.ParseAttributeArgumentList($"(\"Guid\", \"{guid}\")")));

			var newRoot = root.ReplaceNode(classDeclaration, newSyntax);
			var newDocument = document.WithSyntaxRoot(newRoot);
			return Task.FromResult(newDocument);
		}
	}
}
