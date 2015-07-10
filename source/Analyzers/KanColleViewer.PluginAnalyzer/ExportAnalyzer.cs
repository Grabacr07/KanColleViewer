using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Grabacr07.KanColleViewer.PluginAnalyzer
{
	/// <summary>
	/// プラグイン インターフェイスとそれに対応する Export 属性が正しく実装されているかどうかを検出する DiagnosticAnalyzer です。
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ExportAnalyzer : DiagnosticAnalyzer
	{
		public static readonly string DiagnosticId 
			= "KanColleViewer.PluginAnalyzer.Export";

		internal static readonly LocalizableString ExportMessageFormat
			= new LocalizableResourceString(nameof(Resources.ExportMessageFormat), Resources.ResourceManager, typeof(Resources));
		
		internal const string Category = "Compiler";

		internal static DiagnosticDescriptor ExportRule
			= new DiagnosticDescriptor(DiagnosticId, ExportMessageFormat, ExportMessageFormat, Category, DiagnosticSeverity.Warning, true);
		
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ExportRule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
		}

		private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
		{
			var classDeclaration = context.Node as ClassDeclarationSyntax;

			if (classDeclaration.GetMissingExports().Any()
				|| classDeclaration.GetMissingInterfaces().Any())
				context.ReportDiagnostic(Diagnostic.Create(ExportRule, classDeclaration.GetLocation()));
		}
	}
}
