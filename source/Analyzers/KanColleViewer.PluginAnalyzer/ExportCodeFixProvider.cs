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
	/// プラグイン インターフェイスとそれに対応する Export 属性が正しく実装されていない場合にコード修正を行う CodeFixProvider です。
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExportCodeFixProvider)), Shared]
	public class ExportCodeFixProvider : CodeFixProvider
	{
		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ExportAnalyzer.DiagnosticId);

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

			var missingInterfaces = classDeclaration.GetMissingInterfaces();
			if (missingInterfaces.Any())
			{
				var codeAction = CodeAction.Create(
					"Add Missing Interfaces",
					_ => AddInterface(context.Document, root, classDeclaration, missingInterfaces),
					$"{ExportAnalyzer.DiagnosticId} Add Missing Interfaces");
				context.RegisterCodeFix(codeAction, diagnostic);
			}

			var missingExports = classDeclaration.GetMissingExports();
			if (missingExports.Any())
			{
				var codeAction = CodeAction.Create(
					"Add Missing Exports",
					_ => AddExport(context.Document, root, classDeclaration, missingExports),
					$"{ExportAnalyzer.DiagnosticId} Add Missing Exports");
				context.RegisterCodeFix(codeAction, diagnostic);
			}
		}

		private static Task<Document> AddInterface(Document document, CompilationUnitSyntax root, ClassDeclarationSyntax classDeclaration, string[] interfaceNames)
		{
			var newSyntax = classDeclaration;
			foreach (var interfaceName in interfaceNames)
			{
				var baseType = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(interfaceName));
				newSyntax = newSyntax
					.AddBaseListTypes(baseType);
				newSyntax = newSyntax.ReplaceToken(newSyntax.Identifier,
					newSyntax.Identifier.WithTrailingTrivia(SyntaxFactory.Whitespace(" ")));
			}
			var newRoot = root.ReplaceNode(classDeclaration, newSyntax);
			var newDocument = document.WithSyntaxRoot(newRoot);
			return Task.FromResult(newDocument);
		}

		private static Task<Document> AddExport(Document document, CompilationUnitSyntax root, ClassDeclarationSyntax classDeclaration, string[] interfaceNames)
		{
			var newSyntax = classDeclaration;
			foreach (var interfaceName in interfaceNames)
			{
				var attributeList = SyntaxFactory.AttributeList(
					SyntaxFactory.SingletonSeparatedList(
						SyntaxFactory.Attribute(SyntaxFactory.ParseName("Export"),
							SyntaxFactory.ParseAttributeArgumentList($"(typeof({interfaceName}))"))));
				newSyntax = newSyntax.AddAttributeLists(attributeList);
			}
			var newRoot = root.ReplaceNode(classDeclaration, newSyntax);
			var newDocument = document.WithSyntaxRoot(newRoot);
			return Task.FromResult(newDocument);
		}
	}
}
