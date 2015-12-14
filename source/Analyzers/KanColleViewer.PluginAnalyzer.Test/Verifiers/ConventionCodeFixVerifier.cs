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
using System;

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
            var expectedResults = GetDiagnosticResult(ReadResults(resultsPath)).ToArray();

            var expectedSources = new Dictionary<string, string>();
            var expectedSourcePath = Path.Combine(DataSourcePath, testName, "NewSource.cs");
            if (File.Exists(expectedSourcePath)) { expectedSources.Add(Path.GetFileName(sourcePath), File.ReadAllText(expectedSourcePath)); }

            VerifyCSharp(sources, expectedResults, new FixResult(0, expectedSources));
        }

        #endregion
        #region Ver. 2

        private void VerifyCSharpByConventionV2(string testName)
        {
            var sources = ReadSources(testName);
            var expectedResults = ReadDiagnosticResultsFromFolder(testName);
            var expectedSources = ReadExpectedSources(testName);

            VerifyCSharp(sources, expectedResults.ToArray(), expectedSources.ToArray());
        }

        private IEnumerable<DiagnosticResult> ReadDiagnosticResultsFromFolder(string testName)
        {
            var diagnosticPath = Path.Combine(DataSourcePath, testName, "Diagnostic");

            if (!Directory.Exists(diagnosticPath))
                return Array.Empty<DiagnosticResult>();

            var results = ReadResultsFromFolder(diagnosticPath);

            return GetDiagnosticResult(results);
        }

        private IEnumerable<Result> ReadResultsFromFolder(string diagnosticPath)
        {
            foreach (var file in Directory.GetFiles(diagnosticPath, "*.json"))
            {
                if (file.EndsWith("action.json", System.StringComparison.InvariantCultureIgnoreCase))
                    continue;

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

        private IEnumerable<FixResult> ReadExpectedSources(string testName)
        {
            var testPath = Path.Combine(DataSourcePath, testName);

            var exprectedFolders = Directory.GetDirectories(testPath, "Expected*");

            foreach (var expectedPath in exprectedFolders)
            {
                var m = System.Text.RegularExpressions.Regex.Match(expectedPath, @"\d+$");
                var index = m.Success ? int.Parse(m.Value) : 0;

                yield return new FixResult(index, ReadFiles(expectedPath));
            }
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

        class FixResult
        {
            public int Index { get; }

            public Dictionary<string, string> ExpectedSources { get; }

            /// <summary></summary>
            /// <param name="index"><see cref="Index"/></param>
            /// <param name="expectedSources"><see cref="ExpectedSources"/></param>
            public FixResult(int index, Dictionary<string, string> expectedSources)
            {
                Index = index;
                ExpectedSources = expectedSources;
            }
        }

        private void VerifyCSharp(Dictionary<string, string> sources, DiagnosticResult[] expectedResults, params FixResult[] fixResults)
        {
            var analyzer = GetCSharpDiagnosticAnalyzer();
            var fix = GetCSharpCodeFixProvider();

            var originalProject = CreateProject(sources);

            var diagnostics = GetDiagnostics(originalProject, analyzer);
            VerifyDiagnosticResults(diagnostics, analyzer, expectedResults);

            foreach (var fixResult in fixResults)
            {
                var project = ApplyFix(originalProject, analyzer, fix, fixResult.Index);

                var expectedSources = fixResult.ExpectedSources;

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
        }

        private static Project ApplyFix(Project project, DiagnosticAnalyzer analyzer, CodeFixProvider fix, int fixIndex)
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

                var codeAction = actions[fixIndex];

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

        private IEnumerable<DiagnosticResult> GetDiagnosticResult(IEnumerable<Result> results)
        {
            var supportedDiagnostics = GetCSharpDiagnosticAnalyzer().SupportedDiagnostics;
            var analyzers = supportedDiagnostics.ToDictionary(x => x.Id);

            foreach (var r in results)
            {
                var diag = analyzers[r.Id];
                yield return new DiagnosticResult
                {
                    Id = r.Id,
                    Message = r.MessageArgs == null ? diag.MessageFormat.ToString() : string.Format(diag.MessageFormat.ToString(), (object[])r.MessageArgs),
                    Severity = r.Sevirity,
                    Locations = new[] { new DiagnosticResultLocation(r.Path ?? "Source.cs", r.Line, r.Column) },
                };
            }
        }

        private IEnumerable<Result> ReadResults(string path)
        {
            if (!File.Exists(path)) return Array.Empty<Result>();

            try
            {
                var result = JsonConvert.DeserializeObject<Result>(File.ReadAllText(path));
                return new[] { result };
            }
            catch
            {
            }

            // backward compatibility
            try
            {
                var results = JsonConvert.DeserializeObject<Result[]>(File.ReadAllText(path));
                return results;
            }
            catch
            {
            }

            return Array.Empty<Result>();
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
