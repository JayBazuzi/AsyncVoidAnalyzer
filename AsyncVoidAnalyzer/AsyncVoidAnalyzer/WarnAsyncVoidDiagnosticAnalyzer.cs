using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AsyncVoidAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WarnAsyncVoidDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public static readonly DiagnosticDescriptor DiagnosticDescriptor = new DiagnosticDescriptor(
            "ASYNCVOID0",
            "Don't use `async void`.",
            "'{0}' is `async void`.",
            "Category: Async Void",
            DiagnosticSeverity.Warning,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(new AsyncVoidMethodAnalyzer().Analyze,
                SymbolKind.Method);
            context.RegisterSyntaxNodeAction(new AsyncVoidLambdaAnalyzer().Analyze,
                SyntaxKind.ParenthesizedLambdaExpression,
                SyntaxKind.SimpleLambdaExpression,
                SyntaxKind.AnonymousMethodExpression);
        }

        public static Diagnostic CreateDiagnostic(DiagnosticDescriptor diagnosticDescriptor,
            IMethodSymbol methodSymbol)
        {
            return Diagnostic.Create(diagnosticDescriptor, methodSymbol.Locations.Single(), methodSymbol);
        }

        public class AsyncVoidLambdaAnalyzer
        {
            public void Analyze(SyntaxNodeAnalysisContext context)
            {
                var methodSymbol =
                    (IMethodSymbol) context.SemanticModel.GetSymbolInfo((ExpressionSyntax) context.Node).Symbol;

                if (methodSymbol.IsAsync && methodSymbol.ReturnsVoid)
                {
                    context.ReportDiagnostic(CreateDiagnostic(DiagnosticDescriptor, methodSymbol));
                }
            }
        }

        public class AsyncVoidMethodAnalyzer
        {
            public void Analyze(SymbolAnalysisContext context)
            {
                var methodSymbol = (IMethodSymbol) context.Symbol;

                if (methodSymbol.IsAsync && methodSymbol.ReturnsVoid)
                {
                    context.ReportDiagnostic(CreateDiagnostic(DiagnosticDescriptor, methodSymbol));
                }
            }
        }
    }
}