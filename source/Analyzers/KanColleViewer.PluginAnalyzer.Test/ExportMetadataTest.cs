using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;

namespace Grabacr07.KanColleViewer.PluginAnalyzer.Test
{
	[TestClass]
	public class ExportMetadataTest : CodeFixVerifier
	{
        private static readonly string exportMetadataMessage = "ExportMetadata 属性が不足しています。";

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
			for (var i = 0; i < 200; i++) this.DefinedMetadata();
			Assert.IsTrue(sw.ElapsedMilliseconds < 2000);
		}

		[TestMethod]
		public void DefinedMetadata()
		{
			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
[ExportMetadata(""Title"", """")]
[ExportMetadata(""Description"", """")]
[ExportMetadata(""Version"", """")]
[ExportMetadata(""Author"", """")]
class TypeName : IPlugin
{
	public void Initialize() { }
}";
			this.VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void MissingMetadata()
		{
			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
class TypeName : IPlugin
{
	public void Initialize() { }
}";
			var expected = new DiagnosticResult
			{
				Id = ExportMetadataAnalyzer.DiagnosticId,
				Severity = DiagnosticSeverity.Warning,
				Message = exportMetadataMessage,
				Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) }
			};
			this.VerifyCSharpDiagnostic(test, expected);

			var fixedtest = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
[ExportMetadata(""Title"", """")]
[ExportMetadata(""Description"", """")]
[ExportMetadata(""Version"", """")]
[ExportMetadata(""Author"", """")]
class TypeName : IPlugin
{
	public void Initialize() { }
}";
			this.VerifyCSharpFix(test, fixedtest);
		}
		
		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new ExportMetadataCodeFixProvider();
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new ExportMetadataAnalyzer();
		}
	}
}
