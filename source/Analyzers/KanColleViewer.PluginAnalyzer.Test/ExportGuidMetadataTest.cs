using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Grabacr07.KanColleViewer.PluginAnalyzer.Test
{
	[TestClass]
	public class ExportGuidMetadataTest : CodeFixVerifier
	{
		private static readonly string exportGuidMetadataMessage = "GUID の ExportMetadata 属性が不足しています。";

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
			for (var i = 0; i < 200; i++) this.DefinedGuid();
			Assert.IsTrue(sw.ElapsedMilliseconds < 2000);
		}

		[TestMethod]
		public void DefinedGuid()
		{
			Utility.NewGuidValueForTest = Const.NewGuidValueForTest;

			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"")]
class TypeName : ITool
{
	public void Initialize() { }
}";
			this.VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void MissingGuidWhenFeature()
		{
			Utility.NewGuidValueForTest = Const.NewGuidValueForTest;

			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(ITool))]
class TypeName : ITool
{
	public void Initialize() { }
}";
			var expected = new DiagnosticResult
			{
				Id = ExportGuidMetadataAnalyzer.DiagnosticId,
				Severity = DiagnosticSeverity.Warning,
				Message = exportGuidMetadataMessage,
				Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) }
			};
			this.VerifyCSharpDiagnostic(test, expected);

			var fixedtest = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"")]
class TypeName : ITool
{
	public void Initialize() { }
}";
			this.VerifyCSharpFix(test, fixedtest);
		}

		[TestMethod]
		public void MissingGuidWhenFeatureWithIPlugin()
		{
			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(ITool))]
class TypeName : ITool
{
	public void Initialize() { }
}
[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""3F2504E0-4F89-11D3-9A0C-0305E82C3301"")]
class TypeName : IPlugin
{
	public void Initialize() { }
}";
			var expected = new DiagnosticResult
			{
				Id = ExportGuidMetadataAnalyzer.DiagnosticId,
				Severity = DiagnosticSeverity.Warning,
				Message = exportGuidMetadataMessage,
				Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) }
			};
			this.VerifyCSharpDiagnostic(test, expected);

			var fixedtest = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""3F2504E0-4F89-11D3-9A0C-0305E82C3301"")]
class TypeName : ITool
{
	public void Initialize() { }
}
[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""3F2504E0-4F89-11D3-9A0C-0305E82C3301"")]
class TypeName : IPlugin
{
	public void Initialize() { }
}";
			this.VerifyCSharpFix(test, fixedtest);
		}

		[TestMethod]
		public void MissingGuidWhenIPlugin()
		{
			Utility.NewGuidValueForTest = Const.NewGuidValueForTest;

			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
class TypeName : IPlugin
{
	public void Initialize() { }
}";
			var expected = new DiagnosticResult
			{
				Id = ExportGuidMetadataAnalyzer.DiagnosticId,
				Severity = DiagnosticSeverity.Warning,
				Message = exportGuidMetadataMessage,
				Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) }
			};
			this.VerifyCSharpDiagnostic(test, expected);

			var fixedtest = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"")]
class TypeName : IPlugin
{
	public void Initialize() { }
}";
			this.VerifyCSharpFix(test, fixedtest);
		}

		[TestMethod]
		public void MissingGuidWhenIPluginWithUnusedGuid()
		{
			Utility.NewGuidValueForTest = Const.NewGuidValueForTest;

			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
class TypeName : IPlugin
{
	public void Initialize() { }
}
[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""3F2504E0-4F89-11D3-9A0C-0305E82C3301"")]
class TypeName : ITool
{
	public void Initialize() { }
}";
			var expected = new DiagnosticResult
			{
				Id = ExportGuidMetadataAnalyzer.DiagnosticId,
				Severity = DiagnosticSeverity.Warning,
				Message = exportGuidMetadataMessage,
				Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) }
			};
			this.VerifyCSharpDiagnostic(test, expected);

			var fixedtest = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""3F2504E0-4F89-11D3-9A0C-0305E82C3301"")]
class TypeName : IPlugin
{
	public void Initialize() { }
}
[Export(typeof(ITool))]
[ExportMetadata(""Guid"", ""3F2504E0-4F89-11D3-9A0C-0305E82C3301"")]
class TypeName : ITool
{
	public void Initialize() { }
}";
			this.VerifyCSharpFix(test, fixedtest, 1);
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new ExportGuidMetadataCodeFixProvider();
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new ExportGuidMetadataAnalyzer();
		}
	}
}
