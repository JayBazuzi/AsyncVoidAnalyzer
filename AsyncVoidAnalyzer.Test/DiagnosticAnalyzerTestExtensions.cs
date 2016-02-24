using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace AsyncVoidAnalyzer.Test.DiagnosticAnalyzerTestExtensions
{
    internal static class _
    {
        public static async Task<IEnumerable<Diagnostic>> ApplyDiagnostics(this DiagnosticAnalyzer analyzer,
            SourceText sourceText)
        {
            var compilation = await CreateProject("Test.cs", sourceText).GetCompilationAsync();
            compilation.GetDiagnostics().Where(_ => _.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

            var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create(analyzer));
            return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
        }

        private static Project CreateProject(string sourceFileName, SourceText sourceText)
        {
            var projectId = ProjectId.CreateNewId(sourceFileName);
            var documentId = DocumentId.CreateNewId(projectId, sourceFileName);

            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, sourceFileName, sourceFileName, LanguageNames.CSharp)
                .AddMetadataReference(projectId, CreateMetadataReferenceFromType(typeof (object), "mscorlib.dll"))
                .AddMetadataReference(projectId, CreateMetadataReferenceFromType(typeof (Enumerable), "System.Core.dll"))
                .AddDocument(documentId, sourceFileName, sourceText);

            var project = solution.GetProject(projectId);

            return project.WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        private static PortableExecutableReference CreateMetadataReferenceFromType(Type type, string assemblyFileName)
        {
            var reference = MetadataReference.CreateFromFile(type.Assembly.Location);
            reference.Display.Should().EndWith(assemblyFileName);
            return reference;
        }
    }
}