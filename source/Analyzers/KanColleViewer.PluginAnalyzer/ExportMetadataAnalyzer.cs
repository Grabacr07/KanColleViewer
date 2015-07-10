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
	/// 定義が不足している ExportMetadata 属性を検出する DiagnosticAnalyzer です。
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ExportMetadataAnalyzer : DiagnosticAnalyzer
	{
		public static readonly string DiagnosticId
			= "KanColleViewer.PluginAnalyzer.ExportMetadata";

		internal static readonly LocalizableString ExportMetadataMessageFormat
			= new LocalizableResourceString(nameof(Resources.ExportMetadataMessageFormat), Resources.ResourceManager, typeof(Resources));

		internal const string Category = "Compiler";

		internal static DiagnosticDescriptor ExportMetadataRule
			= new DiagnosticDescriptor(DiagnosticId, ExportMetadataMessageFormat, ExportMetadataMessageFormat, Category, DiagnosticSeverity.Warning, true);
		
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ExportMetadataRule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
		}

		private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
		{
			var classDeclaration = context.Node as ClassDeclarationSyntax;

			if (classDeclaration.GetMissingMetadatas().Any())
				context.ReportDiagnostic(Diagnostic.Create(ExportMetadataRule, classDeclaration.GetLocation()));
		}
	}
}
