using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Grabacr07.KanColleViewer.PluginAnalyzer.Test
{
	[TestClass]
	public class DuplicateAttributeTest : CodeFixVerifier
	{
		private static readonly string exportMessage = "{0} の Export 属性が重複しています。";
		private static readonly string metadataMessage = "{0} の ExportMetadata 属性が重複しています。";

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
			for (var i = 0; i < 200; i++) this.NoDuplicate();
			Assert.IsTrue(sw.ElapsedMilliseconds < 2000);
		}


		[TestMethod]
		public void NoDuplicate()
		{
			Utility.NewGuidValueForTest = Const.NewGuidValueForTest;

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
		public void DuplicateAttribute()
		{
			Utility.NewGuidValueForTest = Const.NewGuidValueForTest;

			var test = @"
using System;
using System.ComponentModel.Composition;
using Grabacr07.KanColleViewer.Composition;

[Export(typeof(IPlugin))]
[Export(typeof(IPlugin))]
[Export(typeof(ISettings))]
[Export(typeof(ISettings))]
[Export(typeof(ISettings))]
[ExportMetadata(""Guid"", ""45BF5FE6-7D81-4978-8B8A-84FD80BBEC10"")]
[ExportMetadata(""Title"", """")]
[ExportMetadata(""Description"", """")]
[ExportMetadata(""Version"", """")]
[ExportMetadata(""Version"", """")]
[ExportMetadata(""Author"", """")]
[ExportMetadata(""Author"", """")]
class TypeName : IPlugin, ISettings
{
	public void Initialize() { }
	public object View { get; }
}";
			var expected = new[]
			{
				new DiagnosticResult
				{
					Id = DuplicateAttributeAnalyzer.DiagnosticId,
					Severity = DiagnosticSeverity.Warning,
					Message = string.Format(exportMessage, "typeof(IPlugin), typeof(ISettings)"),
					Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) }
				},
				new DiagnosticResult
				{
					Id = DuplicateAttributeAnalyzer.DiagnosticId,
					Severity = DiagnosticSeverity.Warning,
					Message = string.Format(metadataMessage, "Version, Author"),
					Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 1) }
				},
			};
			this.VerifyCSharpDiagnostic(test, expected);
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return null;
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new DuplicateAttributeAnalyzer();
		}
	}
}
