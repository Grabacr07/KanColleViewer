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
	/// GUID の ExportMetadata 属性が定義されているかどうかを検出する DiagnosticAnalyzer です。
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class ExportGuidMetadataAnalyzer : DiagnosticAnalyzer
	{
		public static readonly string DiagnosticId
			= "KanColleViewer.PluginAnalyzer.ExportGuidMetadata";

		internal static readonly LocalizableString ExportGuidMetadataMessageFormat
			= new LocalizableResourceString(nameof(Resources.ExportGuidMetadataMessageFormat), Resources.ResourceManager, typeof(Resources));

		internal const string Category = "Compiler";

		internal static DiagnosticDescriptor ExportGuidMetadataRule
			= new DiagnosticDescriptor(DiagnosticId, ExportGuidMetadataMessageFormat, ExportGuidMetadataMessageFormat, Category, DiagnosticSeverity.Warning, true);
		
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ExportGuidMetadataRule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
		}

		private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
		{
			var classDeclaration = context.Node as ClassDeclarationSyntax;

			if (classDeclaration.RequireGuidMetadata())
				context.ReportDiagnostic(Diagnostic.Create(ExportGuidMetadataRule, classDeclaration.GetLocation()));
		}
	}
}
