using AsyncVoidAnalyzer.Test.DiagnosticAnalyzerTestExtensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AsyncVoidAnalyzer.Test
{
    [TestClass]
    public class Implicit_async_void_should_trigger_a_warning
    {
        [TestMethod]
        public void FailLambda()
        {
            var testSubject = new WarnAsyncVoidDiagnosticAnalyzer();

            var result = testSubject.ApplyDiagnostics(SourceText.From(@"
class C
{
    void F(System.Action _)
    {
        F(async () => {});
    }
}
")).Result;

            result.Should().ContainSingle()
                .Which.Descriptor.Should().Be(WarnAsyncVoidDiagnosticAnalyzer.DiagnosticDescriptor);
        }


        [TestMethod]
        public void FailAnonymousMethod()
        {
            var testSubject = new WarnAsyncVoidDiagnosticAnalyzer();

            var result = testSubject.ApplyDiagnostics(SourceText.From(@"
class C
{
    void F(System.Action _)
    {
        F(async delegate { });
    }
}
")).Result;


            result.Should().ContainSingle()
                .Which.Descriptor.Should().Be(WarnAsyncVoidDiagnosticAnalyzer.DiagnosticDescriptor);
        }

        [TestMethod]
        public void PassLambda()
        {
            var testSubject = new WarnAsyncVoidDiagnosticAnalyzer();

            var result = testSubject.ApplyDiagnostics(SourceText.From(@"
class C
{
    void F(System.Func<System.Threading.Tasks.Task> _)
    {
        F(async () => {});
    }
}
")).Result;

            result.Should().BeEmpty();
        }
        [TestMethod]
        public void PassAnonymousMethod()
        {
            var testSubject = new WarnAsyncVoidDiagnosticAnalyzer();

            var result = testSubject.ApplyDiagnostics(SourceText.From(@"
class C
{
    void F(System.Func<System.Threading.Tasks.Task> _)
    {
        F(async delegate {});
    }
}
")).Result;

            result.Should().BeEmpty();
        }
    }
}