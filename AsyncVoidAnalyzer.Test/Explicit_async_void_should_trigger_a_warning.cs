using AsyncVoidAnalyzer.Test.DiagnosticAnalyzerTestExtensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AsyncVoidAnalyzer.Test
{
    [TestClass]
    public class Explicit_async_void_should_trigger_a_warning
    {
        [TestMethod]
        public void Pass()
        {
            var testSubject = new WarnAsyncVoidDiagnosticAnalyzer();

            var result = testSubject.ApplyDiagnostics(SourceText.From(@"
class C
{
     void F(){}
}
")).Result;

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void Fail()
        {
            var testSubject = new WarnAsyncVoidDiagnosticAnalyzer();

            var result = testSubject.ApplyDiagnostics(SourceText.From(@"
class C
{
    async void F(){}
}
")).Result;

            result.Should().ContainSingle()
                .Which.Descriptor.Should().Be(WarnAsyncVoidDiagnosticAnalyzer.DiagnosticDescriptor);
        }
    }
}