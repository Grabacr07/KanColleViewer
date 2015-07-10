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
	/// 同じ GUID が定義されている IPlugin 実装が存在する場合にコード修正を行う CodeFixProvider です。
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DuplicateGuidCodeFixProvider)), Shared]
	public class DuplicateGuidCodeFixProvider : CodeFixProvider
	{
		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DuplicateGuidAnalyzer.DiagnosticId);

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

			var codeAction = CodeAction.Create(
				"Use New Guid",
				_ => FixGuidExportMetadata(context.Document, root, classDeclaration, Utility.NewGuidValue()),
				$"{DuplicateGuidAnalyzer.DiagnosticId} Use New Guid");
			context.RegisterCodeFix(codeAction, diagnostic);

			//他のIPluginで使われてないGUIDを持ってるPluginFeatureがいたらそのGUIDを候補に
			var candidates = features.Where(x => plugins.All(y => x.GetGuidMetadataValue() != y.GetGuidMetadataValue()));
			foreach (var candidate in candidates)
			{
				var additionalAction = CodeAction.Create(
					$"Use Guid of {candidate.Identifier}",
					_ => FixGuidExportMetadata(context.Document, root, classDeclaration, candidate.GetGuidMetadataValue()),
					$"{DuplicateGuidAnalyzer.DiagnosticId} Use Guid of {candidate.Identifier}");
				context.RegisterCodeFix(additionalAction, diagnostic);
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
