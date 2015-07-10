using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;

namespace Grabacr07.KanColleViewer.PluginAnalyzer.Test
{
	[TestClass]
	public class InvalidGuidMetadataTest : ConventionCodeFixVerifier
	{
		private static readonly string invalidGuidMessage = "GUID の ExportMetadata 属性には、GUID と解釈できる形式で、 IPlugin インターフェイスを実装したクラスで指定した GUID と同じ値を指定する必要があります。";

		//No diagnostics expected to show up
		[TestMethod]
		public void Empty()
		{
			var test = @"";

			this.VerifyCSharpDiagnostic(test);
		}

		//[TestMethod]
		public void Performance()
		{
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			for (var i = 0; i < 200; i++) this.ValidGuidValue();
			Assert.IsTrue(sw.ElapsedMilliseconds < 2000);
		}

		[TestMethod]
		public void ValidGuidValue()
		{
			Utility.NewGuidValueForTest = Const.NewGuidValueForTest;

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
		public void MissingGuidValue()
		{
			Utility.NewGuidValueForTest = Const.NewGuidValueForTest;

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
[ExportMetadata(""Guid"", """")]
class Fuga : ITool
{
	public void Initialize() { }
}",
			};
			var expected = new DiagnosticResult
			{
				Id = InvalidGuidMetadataAnalyzer.DiagnosticId,
				Severity = DiagnosticSeverity.Warning,
				Message = invalidGuidMessage,
				Locations = new[] { new DiagnosticResultLocation("Test1.cs", 6, 1) }
			};
			this.VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void DifferentFromIPluginGuid()
		{
			this.VerifyCSharpByConvention();
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new InvalidGuidMetadataCodeFixProvider();
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new InvalidGuidMetadataAnalyzer();
		}
	}
}
