using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;

namespace Grabacr07.KanColleViewer.PluginAnalyzer.Test
{
	[TestClass]
	public class ExportTest : CodeFixVerifier
	{
		private static readonly string exportMessage = "プラグインを実装するには、プラグイン インターフェイスの実装と、それに対応する Export 属性の指定を行う必要があります。";

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
			for (var i = 0; i < 200; i++) this.HasExportAndHasInterface();
			Assert.IsTrue(sw.ElapsedMilliseconds < 2000);
		}

		[TestMethod]
		public void HasExportAndHasInterface()
		{
			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
[ExportMetadata(""Title"", ""MastarData"")]
[ExportMetadata(""Description"", ""start2 で取得される、艦これのマスター データを閲覧するためのビュー機能を提供します。"")]
[ExportMetadata(""Version"", ""1.0"")]
[ExportMetadata(""Author"", ""@Grabacr07"")]
class TypeName : IPlugin
{
	public void Initialize() { }
}";
			this.VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void NotHaveExportAndNotHaveInterface()
		{
			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

class TypeName
{
	public void Initialize() { }
}";
			this.VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void NotHaveExportAndHasInterface()
		{
			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
[ExportMetadata(""Title"", ""MastarData"")]
[ExportMetadata(""Description"", ""start2 で取得される、艦これのマスター データを閲覧するためのビュー機能を提供します。"")]
[ExportMetadata(""Version"", ""1.0"")]
[ExportMetadata(""Author"", ""@Grabacr07"")]
class TypeName : IPlugin
{
	public void Initialize() { }
}";
			var expected = new DiagnosticResult
			{
				Id = ExportAnalyzer.DiagnosticId,
				Severity = DiagnosticSeverity.Warning,
				Message = exportMessage,
				Locations = new[] {new DiagnosticResultLocation("Test0.cs", 6, 1)}
			};
			this.VerifyCSharpDiagnostic(test, expected);

			var fixedtest = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
[ExportMetadata(""Title"", ""MastarData"")]
[ExportMetadata(""Description"", ""start2 で取得される、艦これのマスター データを閲覧するためのビュー機能を提供します。"")]
[ExportMetadata(""Version"", ""1.0"")]
[ExportMetadata(""Author"", ""@Grabacr07"")]
[Export(typeof(IPlugin))]
class TypeName : IPlugin
{
	public void Initialize() { }
}";
			this.VerifyCSharpFix(test, fixedtest);
		}


		[TestMethod]
		public void HasExportAndNotHaveInterface()
		{
			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
[ExportMetadata(""Title"", ""MastarData"")]
[ExportMetadata(""Description"", ""start2 で取得される、艦これのマスター データを閲覧するためのビュー機能を提供します。"")]
[ExportMetadata(""Version"", ""1.0"")]
[ExportMetadata(""Author"", ""@Grabacr07"")]
class TypeName
{
	public void Initialize() { }
}";
			var expected = new DiagnosticResult
			{
				Id = ExportAnalyzer.DiagnosticId,
				Severity = DiagnosticSeverity.Warning,
				Message = exportMessage,
				Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) }
			};
			this.VerifyCSharpDiagnostic(test, expected);

			var fixedtest = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
[ExportMetadata(""Title"", ""MastarData"")]
[ExportMetadata(""Description"", ""start2 で取得される、艦これのマスター データを閲覧するためのビュー機能を提供します。"")]
[ExportMetadata(""Version"", ""1.0"")]
[ExportMetadata(""Author"", ""@Grabacr07"")]
class TypeName : IPlugin
{
	public void Initialize() { }
}";
			this.VerifyCSharpFix(test, fixedtest);
		}

		[TestMethod]
		public void InheritExport()
		{
			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[InheritedExport(typeof(INotifier))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class TypeName : INotifier
{
	public void Initialize() { }
}";
			this.VerifyCSharpDiagnostic(test);
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new ExportCodeFixProvider();
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new ExportAnalyzer();
		}
	}
}
