using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Grabacr07.KanColleViewer.PluginAnalyzer.Test
{
	[TestClass]
	public class DuplicateGuidTest : CodeFixVerifier
	{
		private static readonly string message = "既に他の プラグイン {0} で使用されている GUID です。";

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
			for (var i = 0; i < 200; i++) this.NoDuplicateGuid();
			Assert.IsTrue(sw.ElapsedMilliseconds < 2000);
		}


		[TestMethod]
		public void NoDuplicateGuid()
		{
			Utility.NewGuidValueForTest = Const.NewGuidValueForTest;

			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""65BE3E80-8EC1-41BD-85E0-78AEFD45A757"")]
class Hoge : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Fuga : IPlugin
{
	public void Initialize() { }
}";
			this.VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void DuplicateGuid()
		{
			Utility.NewGuidValueForTest = Const.NewGuidValueForTest;

			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""65BE3E80-8EC1-41BD-85E0-78AEFD45A757"")]
class Hoge1 : IPlugin
{
	public void Initialize() { }
}
[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""65BE3E80-8EC1-41BD-85E0-78AEFD45A757"")]
class Hoge2 : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Fuga1 : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Fuga2 : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Fuga3 : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""006130BF-499B-45E5-8EFA-CFFF192D1E65"")]
class PiyoPlugin : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""006130BF-499B-45E5-8EFA-CFFF192D1E65"")]
class Piyo1 : ITool
{
	public void Initialize() { }
}

[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""C78D5FAA-F686-45E6-B986-E4D5393DBE77"")]
class Piyo2 : ITool
{
	public void Initialize() { }
}";
			var expected = new[]
			{
				new DiagnosticResult
				{
					Id = DuplicateGuidAnalyzer.DiagnosticId,
					Severity = DiagnosticSeverity.Warning,
					Message = string.Format(message, "Hoge2"),
					Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) }
				},
				new DiagnosticResult
				{
					Id = DuplicateGuidAnalyzer.DiagnosticId,
					Severity = DiagnosticSeverity.Warning,
					Message = string.Format(message, "Hoge1"),
					Locations = new[] { new DiagnosticResultLocation("Test0.cs", 12, 1) }
				},
				new DiagnosticResult
				{
					Id = DuplicateGuidAnalyzer.DiagnosticId,
					Severity = DiagnosticSeverity.Warning,
					Message = string.Format(message, "Fuga2, Fuga3"),
					Locations = new[] { new DiagnosticResultLocation("Test0.cs", 19, 1) }
				},
				new DiagnosticResult
				{
					Id = DuplicateGuidAnalyzer.DiagnosticId,
					Severity = DiagnosticSeverity.Warning,
					Message = string.Format(message, "Fuga1, Fuga3"),
					Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 1) }
				},
				new DiagnosticResult
				{
					Id = DuplicateGuidAnalyzer.DiagnosticId,
					Severity = DiagnosticSeverity.Warning,
					Message = string.Format(message, "Fuga1, Fuga2"),
					Locations = new[] { new DiagnosticResultLocation("Test0.cs", 33, 1) }
				},
			};
			this.VerifyCSharpDiagnostic(test, expected);

			var fixedtest0 = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"")]
class Hoge1 : IPlugin
{
	public void Initialize() { }
}
[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""65BE3E80-8EC1-41BD-85E0-78AEFD45A757"")]
class Hoge2 : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Fuga1 : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Fuga2 : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Fuga3 : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""006130BF-499B-45E5-8EFA-CFFF192D1E65"")]
class PiyoPlugin : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""006130BF-499B-45E5-8EFA-CFFF192D1E65"")]
class Piyo1 : ITool
{
	public void Initialize() { }
}

[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""C78D5FAA-F686-45E6-B986-E4D5393DBE77"")]
class Piyo2 : ITool
{
	public void Initialize() { }
}";
			this.VerifyCSharpFix(test, fixedtest0, 0);

			var fixedtest1 = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""C78D5FAA-F686-45E6-B986-E4D5393DBE77"")]
class Hoge1 : IPlugin
{
	public void Initialize() { }
}
[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""65BE3E80-8EC1-41BD-85E0-78AEFD45A757"")]
class Hoge2 : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Fuga1 : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Fuga2 : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class Fuga3 : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""006130BF-499B-45E5-8EFA-CFFF192D1E65"")]
class PiyoPlugin : IPlugin
{
	public void Initialize() { }
}

[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""006130BF-499B-45E5-8EFA-CFFF192D1E65"")]
class Piyo1 : ITool
{
	public void Initialize() { }
}

[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""C78D5FAA-F686-45E6-B986-E4D5393DBE77"")]
class Piyo2 : ITool
{
	public void Initialize() { }
}";
			this.VerifyCSharpFix(test, fixedtest1, 1);
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new DuplicateGuidCodeFixProvider();
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new DuplicateGuidAnalyzer();
		}
	}
}
