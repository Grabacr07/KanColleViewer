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
	/// 不正な GUID の ExportMetadata を検出する DiagnosticAnalyzer です。
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class InvalidGuidMetadataAnalyzer : DiagnosticAnalyzer
	{
		public static readonly string DiagnosticId
			= "KanColleViewer.PluginAnalyzer.InvalidGuidMetadata";

		internal static readonly LocalizableString InvalidGuidMetadataMessageFormat
			= new LocalizableResourceString(nameof(Resources.InvalidGuidMetadataMessageFormat), Resources.ResourceManager, typeof(Resources));

		internal const string Category = "Compiler";
		
		internal static DiagnosticDescriptor InvalidGuidMetadataRule
			= new DiagnosticDescriptor(DiagnosticId, InvalidGuidMetadataMessageFormat, InvalidGuidMetadataMessageFormat, Category, DiagnosticSeverity.Warning, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(InvalidGuidMetadataRule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterCompilationAction(AnalyzeCompilation);
		}

		private static void AnalyzeCompilation(CompilationAnalysisContext context)
		{
			var compilation = context.Compilation;
			var allPlugins = compilation.SyntaxTrees
				.Select(x => x.GetCompilationUnitRoot())
				.SelectMany(x => x.FindSyntax<ClassDeclarationSyntax>())
				.Where(x => x.IsPluginClass())
				.ToArray();
			var plugins = allPlugins.Where(x => x.IsExportIPlugin()).ToArray();

			foreach (var p in allPlugins)
			{
				var guidVaue = p.GetGuidMetadataValue();
				if (guidVaue == null) continue;	//GUID な Metadata がない場合はスルー

				// GUID として解釈できない値か、IPlugin で未定義の GUID が指定されてたらアウト
				Guid guid;
				if (!Guid.TryParse(guidVaue, out guid)
					|| plugins.All(x => x.GetGuidMetadataValue().ToUpper() != guidVaue.ToUpper()))
					context.ReportDiagnostic(Diagnostic.Create(InvalidGuidMetadataRule, p.GetLocation()));
			}
		}
	}
}
