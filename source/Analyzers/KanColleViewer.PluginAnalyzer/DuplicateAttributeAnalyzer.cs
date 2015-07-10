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
	/// 重複定義されている Export 属性、ExportMetadata 属性を検出する DiagnosticAnalyzer です。
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class DuplicateAttributeAnalyzer : DiagnosticAnalyzer
	{
		public static readonly string DiagnosticId
			= "KanColleViewer.PluginAnalyzer.DuplicateAttribute";

		internal static readonly LocalizableString DuplicateExportAttributeMessageFormat
			= new LocalizableResourceString(nameof(Resources.DuplicateExportAttributeMessageFormat), Resources.ResourceManager, typeof(Resources));

		internal static readonly LocalizableString DuplicateExportMetadataAttributeMessageFormat
			= new LocalizableResourceString(nameof(Resources.DuplicateExportMetadataAttributeMessageFormat), Resources.ResourceManager, typeof(Resources));

		internal const string Category = "Compiler";

		internal static DiagnosticDescriptor DuplicateExportAttributeRule
			= new DiagnosticDescriptor(DiagnosticId, DuplicateExportAttributeMessageFormat, DuplicateExportAttributeMessageFormat, Category, DiagnosticSeverity.Warning, true);

		internal static DiagnosticDescriptor DuplicateExportMetadataAttributeRule
			= new DiagnosticDescriptor(DiagnosticId, DuplicateExportMetadataAttributeMessageFormat, DuplicateExportMetadataAttributeMessageFormat, Category, DiagnosticSeverity.Warning, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DuplicateExportAttributeRule, DuplicateExportMetadataAttributeRule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
		}

		private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
		{
			var classDeclaration = context.Node as ClassDeclarationSyntax;
			
			var dupeExports = classDeclaration.GetAttributes("Export")
				.GroupBy(x => x.ArgumentList.Arguments.First().ToString())
				.Where(x => 1 < x.Count())
				.Select(x => x.Key.ToString())
				.ToArray();

			if (dupeExports.Any())
				context.ReportDiagnostic(Diagnostic.Create(
					DuplicateExportAttributeRule,
					classDeclaration.GetLocation(),
					string.Join(", ", dupeExports)));


			var dupeMetadatas = classDeclaration.GetAttributes("ExportMetadata")
				.GroupBy(x => x.ArgumentList.Arguments.First().ToString())
				.Where(x => 1 < x.Count())
				.Select(x => x.Key.ToString().Replace("\"", ""))
				.ToArray();

			if (dupeMetadatas.Any())
				context.ReportDiagnostic(Diagnostic.Create(
					DuplicateExportMetadataAttributeRule,
					classDeclaration.GetLocation(),
					string.Join(", ", dupeMetadatas)));
		}
	}
}
