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
	/// 同じ GUID が定義されている IPlugin 実装が存在するかどうかを検出する DiagnosticAnalyzer です。
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class DuplicateGuidAnalyzer : DiagnosticAnalyzer
	{
		public static readonly string DiagnosticId
			= "KanColleViewer.PluginAnalyzer.DuplicateGuid";

		internal static readonly LocalizableString DuplicateGuidMessageFormat
			= new LocalizableResourceString(nameof(Resources.DuplicateGuidMessageFormat), Resources.ResourceManager, typeof(Resources));

		internal const string Category = "Compiler";

		internal static DiagnosticDescriptor DuplicateGuidRule
			= new DiagnosticDescriptor(DiagnosticId, DuplicateGuidMessageFormat, DuplicateGuidMessageFormat, Category, DiagnosticSeverity.Warning, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DuplicateGuidRule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterCompilationAction(AnalyzeCompilation);
		}

		private static void AnalyzeCompilation(CompilationAnalysisContext context)
		{
			var compilation = context.Compilation;
			var plugins = compilation.SyntaxTrees
				.Select(x => x.GetCompilationUnitRoot())
				.SelectMany(x => x.FindSyntax<ClassDeclarationSyntax>())
				.Where(x => x.IsExportIPlugin())
				.ToArray();

			var dupe = plugins
				.GroupBy(x => x.GetGuidMetadataValue())
				.Where(x => 1 < x.Count())
				.ToArray();

			if (!dupe.Any()) return;

			foreach (var group in dupe)
			{
				foreach (var c in group)
				{
					var other = group.Except(new[] { c }).ToArray();
					context.ReportDiagnostic(Diagnostic.Create(
						DuplicateGuidRule,
						c.GetLocation(),
						string.Join(", ", other.Select(x => x.Identifier))));
				}
			}
		}
	}
}
