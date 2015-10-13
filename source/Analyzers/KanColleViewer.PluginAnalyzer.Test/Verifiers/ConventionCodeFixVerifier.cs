using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace TestHelper
{
    public abstract class ConventionCodeFixVerifier : CodeFixVerifier
    {
        public ConventionCodeFixVerifier()
        {
            var t = GetType();
            var path = Path.Combine(t.Assembly.Location, @"../../../DataSource");

            DataSourcePath = Path.Combine(path, t.Name);
        }

        private string DataSourcePath { get; }

        private static string ReadIfExists(string path)
        {
            if (!File.Exists(path)) return string.Empty;
            return File.ReadAllText(path);
        }

        protected void VerifyCSharpByConvention([CallerMemberName]string testName = null)
        {
            var sourcePath = Path.Combine(DataSourcePath, testName, "Source");

            if (Directory.Exists(sourcePath))
            {
                VerifyCSharpByConventionV2(testName);
            }
            else
            {
                VerifyCSharpByConventionV1(testName);
            }
        }

        #region Ver. 1

        /// <summary>
        /// Convention ver. 1.
        /// Load test data from Source.cs, Rsults.json, and NewSource.cs.
        /// </summary>
        /// <param name="testName"></param>
        private void VerifyCSharpByConventionV1(string testName)
        {
            var sources = new Dictionary<string, string>();
            var sourcePath = Path.Combine(DataSourcePath, testName, "Source.cs");
            if (File.Exists(sourcePath)) { sources.Add(Path.GetFileName(sourcePath), File.ReadAllText(sourcePath)); }

            var resultsPath = Path.Combine(DataSourcePath, testName, "Results.json");
            var expectedResults = ReadResults(resultsPath).ToArray();

            var expectedSources = new Dictionary<string, string>();
            var expectedSourcePath = Path.Combine(DataSourcePath, testName, "NewSource.cs");
            if (File.Exists(expectedSourcePath)) { expectedSources.Add(Path.GetFileName(sourcePath), File.ReadAllText(expectedSourcePath)); }

            VerifyCSharp(sources, expectedResults, expectedSources);
        }

        #endregion
        #region Ver. 2

        private void VerifyCSharpByConventionV2(string testName)
        {
            var sources = ReadSources(testName);
            var expectedResults = ReadResultsFromFolder(testName).ToArray();
            var expectedSources = ReadExpectedSources(testName);

            VerifyCSharp(sources, expectedResults, expectedSources);
        }

        private IEnumerable<DiagnosticResult> ReadResultsFromFolder(string testName)
        {
            var diagnosticPath = Path.Combine(DataSourcePath, testName, "Diagnostic");

            if (!Directory.Exists(diagnosticPath))
                yield break;

            foreach (var file in Directory.GetFiles(diagnosticPath, "*.json"))
            {
                foreach (var r in ReadResults(file))
                {
                    yield return r;
                }
            }
        }

        private Dictionary<string, string> ReadSources(string testName)
        {
            var sourcePath = Path.Combine(DataSourcePath, testName, "Source");

            return ReadFiles(sourcePath);
        }

        private Dictionary<string, string> ReadExpectedSources(string testName)
        {
            var expectedPath = Path.Combine(DataSourcePath, testName, "Expected");
            return ReadFiles(expectedPath);
        }

        private static Dictionary<string, string> ReadFiles(string sourcePath)
        {
            if (!Directory.Exists(sourcePath))
                return null; ;

            var sources = new Dictionary<string, string>();

            foreach (var file in Directory.GetFiles(sourcePath, "*.cs"))
            {
                var code = File.ReadAllText(file);
                var name = Path.GetFileName(file);
                sources.Add(name, code);
            }

            return sources;
        }

        private void VerifyCSharp(Dictionary<string, string> sources, DiagnosticResult[] expectedResults, Dictionary<string, string> expectedSources)
        {
            var analyzer = GetCSharpDiagnosticAnalyzer();
            var fix = GetCSharpCodeFixProvider();

            var project = CreateProject(sources);

            var diagnostics = GetDiagnostics(project, analyzer);
            VerifyDiagnosticResults(diagnostics, analyzer, expectedResults);

            project = ApplyFix(project, analyzer, fix);

            if (expectedSources == null || !expectedSources.Any())
                return;

            var actualSources = new Dictionary<string, string>();

            foreach (var doc in project.Documents)
            {
                var code = GetStringFromDocument(doc);
                actualSources.Add(doc.Name, code);
            }

            Assert.IsTrue(actualSources.Keys.SequenceEqual(expectedSources.Keys));

            foreach (var item in actualSources)
            {
                var actual = item.Value;
                var newSource = expectedSources[item.Key];
                Assert.AreEqual(newSource, actual);
            }
        }

        private static Project ApplyFix(Project project, DiagnosticAnalyzer analyzer, CodeFixProvider fix)
        {
            var diagnostics = GetDiagnostics(project, analyzer);

            var fixableDiagnostics = diagnostics.Where(d => fix.FixableDiagnosticIds.Contains(d.Id)).ToArray();

            var attempts = fixableDiagnostics.Length;

            for (int i = 0; i < attempts; i++)
            {
                var diag = fixableDiagnostics.First();
                var doc = project.Documents.FirstOrDefault(d => d.Name == diag.Location.SourceTree.FilePath);

                if (doc == null)
                {
                    fixableDiagnostics = fixableDiagnostics.Skip(1).ToArray();
                    continue;
                }

                var actions = new List<CodeAction>();
                var fixContex = new CodeFixContext(doc, diag, (a, d) => actions.Add(a), CancellationToken.None);
                fix.RegisterCodeFixesAsync(fixContex).Wait();

                if (!actions.Any())
                {
                    break;
                }

                var codeAction = actions[0];

                var operations = codeAction.GetOperationsAsync(CancellationToken.None).Result;
                var solution = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
                project = solution.GetProject(project.Id);

                fixableDiagnostics = GetDiagnostics(project, analyzer)
                    .Where(d => fix.FixableDiagnosticIds.Contains(d.Id)).ToArray();

                if (!fixableDiagnostics.Any()) break;
            }

            return project;
        }

        private static Diagnostic[] GetDiagnostics(Project project, DiagnosticAnalyzer analyzer)
        {
            var compilationWithAnalyzers = project.GetCompilationAsync().Result.WithAnalyzers(ImmutableArray.Create(analyzer));
            var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;
            return diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
        }

        protected virtual IEnumerable<MetadataReference> References
        {
            get
            {
                yield return MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
                yield return MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
                yield return MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
                yield return MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);
            }
        }

        protected virtual CSharpCompilationOptions CompilationOptions => new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        protected Project CreateProject(Dictionary<string, string> sources)
        {
            string fileNamePrefix = DefaultFilePathPrefix;
            string fileExt = CSharpDefaultFileExt;

            var projectId = ProjectId.CreateNewId(debugName: TestProjectName);

            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, LanguageNames.CSharp);

            foreach (var reference in References)
            {
                solution = solution.AddMetadataReference(projectId, reference);
            }

            int count = 0;
            foreach (var source in sources)
            {
                var newFileName = source.Key;
                var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source.Value));
                count++;
            }

            var project = solution.GetProject(projectId)
                .WithCompilationOptions(CompilationOptions);
            return project;
        }

        #endregion
        #region read expected results from JSON file

        private IEnumerable<DiagnosticResult> ReadResults(string path)
        {
            if (!File.Exists(path)) yield break;

            var results = JsonConvert.DeserializeObject<Result[]>(File.ReadAllText(path));

            var analyzer = GetCSharpDiagnosticAnalyzer().SupportedDiagnostics.ToDictionary(x => x.Id);

            foreach (var r in results)
            {
                var diag = analyzer[r.Id];
                yield return new DiagnosticResult
                {
                    Id = r.Id,
                    Message = r.MessageArgs == null ? diag.MessageFormat.ToString() : string.Format(diag.MessageFormat.ToString(), (object[])r.MessageArgs),
                    Severity = r.Sevirity,
                    Locations = new[] { new DiagnosticResultLocation(r.Path ?? "Source.cs", r.Line, r.Column) },
                };
            }
        }

        private class Result
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }

            [JsonProperty(PropertyName = "sevirity")]
            public DiagnosticSeverity Sevirity { get; set; }

            [JsonProperty(PropertyName = "line")]
            public int Line { get; set; }

            [JsonProperty(PropertyName = "column")]
            public int Column { get; set; }

            [JsonProperty(PropertyName = "path")]
            public string Path { get; set; }

            [JsonProperty(PropertyName = "message-args")]
            public string[] MessageArgs { get; set; }
        }

        #endregion
    }
}
