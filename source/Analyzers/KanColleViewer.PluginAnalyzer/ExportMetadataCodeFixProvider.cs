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
	/// 定義が不足している ExportMetadata 属性を検出した場合にコード修正を行う CodeFixProvider です。
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExportMetadataCodeFixProvider)), Shared]
	public class ExportMetadataCodeFixProvider : CodeFixProvider
	{
		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ExportMetadataAnalyzer.DiagnosticId);

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
			
			var missingMetadata = classDeclaration.GetMissingMetadatas();
			if (missingMetadata.Any())
			{
				var codeAction = CodeAction.Create(
					"Add Missing ExportMetadata",
					_ => AddExportMetadata(context.Document, root, classDeclaration, missingMetadata),
					$"{ExportMetadataAnalyzer.DiagnosticId} Add Missing ExportMetadata");
				context.RegisterCodeFix(codeAction, diagnostic);
			}
		}

		private static Task<Document> AddExportMetadata(Document document, CompilationUnitSyntax root, ClassDeclarationSyntax classDeclaration, string[] missingMetadata)
		{
			var newSyntax = classDeclaration;
			//値は空の状態で、必要なExportMetadataを全部追加
			foreach (var metadata in missingMetadata)
			{
				var attributeList = SyntaxFactory.AttributeList(
					SyntaxFactory.SingletonSeparatedList(
						SyntaxFactory.Attribute(SyntaxFactory.ParseName("ExportMetadata"),
							SyntaxFactory.ParseAttributeArgumentList($"(\"{metadata}\", \"\")"))));
				newSyntax = newSyntax.AddAttributeLists(attributeList);
			}
			var newRoot = root.ReplaceNode(classDeclaration, newSyntax);
			var newDocument = document.WithSyntaxRoot(newRoot);
			return Task.FromResult(newDocument);
		}
	}
}
