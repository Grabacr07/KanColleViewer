using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;

namespace Grabacr07.KanColleViewer.PluginAnalyzer.Test
{
	[TestClass]
	public class RequireIPluginTest : CodeFixVerifier
	{
		private static readonly string message = "プラグインを実装するには、IPlugin インターフェイスを実装したクラスが必要です。";

		//No diagnostics expected to show up
		[TestMethod]
		public void Empty()
		{
			var test = @"";

			var expected = new DiagnosticResult
			{
				Id = RequireIPluginAnalyzer.DiagnosticId,
				Severity = DiagnosticSeverity.Error,
				Message = message,
			};
			this.VerifyCSharpDiagnostic(test, expected);
		}

		//[TestMethod]
		public void Performance()
		{
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			for (var i = 0; i < 200; i++) this.DefinedIPlugin();
			Assert.IsTrue(sw.ElapsedMilliseconds < 2000);
		}

		[TestMethod]
		public void DefinedIPlugin()
		{
			var test = new[] {
@"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
[ExportMetadata(""Title"", ""MastarData"")]
[ExportMetadata(""Description"", ""start2 で取得される、艦これのマスター データを閲覧するためのビュー機能を提供します。"")]
[ExportMetadata(""Version"", ""1.0"")]
[ExportMetadata(""Author"", ""@Grabacr07"")]
class Hoge : IPlugin
{
	public void Initialize() { }
}",
@"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Fuga : ITool
{
	public void Initialize() { }
}",
			};
			this.VerifyCSharpDiagnostic(test);
		}


		[TestMethod]
		public void RequireIPlugin()
		{
			var test = new[] {
@"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(ISettings))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Hoge : ISettings
{
	public void Initialize() { }
}",
@"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Fuga : ITool
{
	public void Initialize() { }
}",
			};
			var expected = new DiagnosticResult
			{
				Id = RequireIPluginAnalyzer.DiagnosticId,
				Severity = DiagnosticSeverity.Error,
				Message = message,
			};
			this.VerifyCSharpDiagnostic(test, expected);
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return null;
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new RequireIPluginAnalyzer();
		}
	}
}
