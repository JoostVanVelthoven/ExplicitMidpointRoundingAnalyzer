using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace ExplicitMidpointRoundingAnalyzer.Tests;

using Verify = CSharpCodeFixVerifier<
    ExplicitMidpointRoundingAnalyzer,
    ExplicitMidpointRoundingCodeFixProvider>;

public sealed class ExplicitMidpointRoundingAnalyzerTests
{
    [Fact]
    public async Task MathRoundValueReportsDiagnosticAndAddsAwayFromZero()
    {
        const string test = """
            using System;

            class C
            {
                double M(double value)
                {
                    return {|#0:Math.Round(value)|};
                }
            }
            """;

        const string fixedCode = """
            using System;

            class C
            {
                double M(double value)
                {
                    return Math.Round(value, MidpointRounding.AwayFromZero);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Math"), fixedCode);
    }

    [Fact]
    public async Task MathRoundValueAndDigitsReportsDiagnosticAndAddsAwayFromZero()
    {
        const string test = """
            using System;

            class C
            {
                double M(double value)
                {
                    return {|#0:Math.Round(value, 2)|};
                }
            }
            """;

        const string fixedCode = """
            using System;

            class C
            {
                double M(double value)
                {
                    return Math.Round(value, 2, MidpointRounding.AwayFromZero);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Math"), fixedCode);
    }

    [Fact]
    public async Task DecimalMathRoundReportsDiagnostic()
    {
        const string test = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return {|#0:Math.Round(value)|};
                }
            }
            """;

        await Verify.VerifyAnalyzerAsync(test, Diagnostic().WithLocation(0).WithArguments("Math"));
    }

    [Fact]
    public async Task MathFRoundReportsDiagnostic()
    {
        const string test = """
            using System;

            class C
            {
                float M(float value)
                {
                    return {|#0:MathF.Round(value)|};
                }
            }
            """;

        await Verify.VerifyAnalyzerAsync(test, Diagnostic().WithLocation(0).WithArguments("MathF"));
    }

    [Fact]
    public async Task ExplicitMidpointRoundingReportsNoDiagnostic()
    {
        const string test = """
            using System;

            class C
            {
                double M(double value)
                {
                    var toEven = Math.Round(value, MidpointRounding.ToEven);
                    var awayFromZero = Math.Round(value, 2, MidpointRounding.AwayFromZero);
                    var mathFToEven = MathF.Round((float)value, MidpointRounding.ToEven);
                    var mathFAwayFromZero = MathF.Round((float)value, 2, MidpointRounding.AwayFromZero);
                    return toEven + awayFromZero + mathFToEven + mathFAwayFromZero;
                }
            }
            """;

        await Verify.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task NonSystemRoundReportsNoDiagnostic()
    {
        const string test = """
            class C
            {
                double M(double value)
                {
                    return Math.Round(value);
                }
            }

            static class Math
            {
                public static double Round(double value) => value;
            }
            """;

        await Verify.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task FullyQualifiedMathRoundReportsDiagnostic()
    {
        const string test = """
            class C
            {
                double M(double value)
                {
                    return {|#0:System.Math.Round(value, 2)|};
                }
            }
            """;

        await Verify.VerifyAnalyzerAsync(test, Diagnostic().WithLocation(0).WithArguments("Math"));
    }

    private static DiagnosticResult Diagnostic()
        => Verify.Diagnostic(ExplicitMidpointRoundingAnalyzer.DiagnosticId);
}
