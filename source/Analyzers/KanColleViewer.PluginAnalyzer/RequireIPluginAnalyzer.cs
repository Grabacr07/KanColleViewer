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
	/// IPlugin インターフェイスを実装しているクラスが存在しない場合、エラーとする DiagnosticAnalyzer です。
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class RequireIPluginAnalyzer : DiagnosticAnalyzer
	{
		public static readonly string DiagnosticId
			= "KanColleViewer.PluginAnalyzer.RequireIPlugin";

		internal static readonly LocalizableString RequireIPluginMessageFormat
			= new LocalizableResourceString(nameof(Resources.RequireIPluginMessageFormat), Resources.ResourceManager, typeof(Resources));

		internal const string Category = "Compiler";

		internal static DiagnosticDescriptor RequireIPluginRule
			= new DiagnosticDescriptor(DiagnosticId, RequireIPluginMessageFormat, RequireIPluginMessageFormat, Category, DiagnosticSeverity.Error, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(RequireIPluginRule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterCompilationAction(AnalyzeCompilation);
		}

		private static void AnalyzeCompilation(CompilationAnalysisContext context)
		{
			var compilation = context.Compilation;
			var isDefined = compilation.SyntaxTrees
				.Select(x => x.GetCompilationUnitRoot())
				.SelectMany(x => x.DescendantNodes().OfType<SimpleBaseTypeSyntax>())
				.Any(x => x.ToString() == "IPlugin" || x.ToString() == "Grabacr07.KanColleViewer.Composition.IPlugin")
				;

			if (!isDefined)
				context.ReportDiagnostic(Diagnostic.Create(RequireIPluginRule, Location.None));
		}
	}
}
