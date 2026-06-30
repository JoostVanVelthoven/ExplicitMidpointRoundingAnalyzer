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
    public async Task MathRoundValueReportsDiagnosticAndAddsToEven()
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
                    return Math.Round(value, MidpointRounding.ToEven);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Math"), fixedCode, codeActionIndex: 0);
    }

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

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Math"), fixedCode, codeActionIndex: 1);
    }

    [Fact]
    public async Task MathRoundValueAndDigitsReportsDiagnosticAndAddsToEven()
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
                    return Math.Round(value, 2, MidpointRounding.ToEven);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Math"), fixedCode, codeActionIndex: 0);
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

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Math"), fixedCode, codeActionIndex: 1);
    }

    [Fact]
    public async Task DecimalMathRoundValueReportsDiagnosticAndAddsToEven()
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

        const string fixedCode = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return Math.Round(value, MidpointRounding.ToEven);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Math"), fixedCode, codeActionIndex: 0);
    }

    [Fact]
    public async Task DecimalMathRoundValueReportsDiagnosticAndAddsAwayFromZero()
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

        const string fixedCode = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return Math.Round(value, MidpointRounding.AwayFromZero);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Math"), fixedCode, codeActionIndex: 1);
    }

    [Fact]
    public async Task DecimalMathRoundValueAndDigitsReportsDiagnosticAndAddsToEven()
    {
        const string test = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return {|#0:Math.Round(value, 2)|};
                }
            }
            """;

        const string fixedCode = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return Math.Round(value, 2, MidpointRounding.ToEven);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Math"), fixedCode, codeActionIndex: 0);
    }

    [Fact]
    public async Task DecimalMathRoundValueAndDigitsReportsDiagnosticAndAddsAwayFromZero()
    {
        const string test = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return {|#0:Math.Round(value, 2)|};
                }
            }
            """;

        const string fixedCode = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return Math.Round(value, 2, MidpointRounding.AwayFromZero);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Math"), fixedCode, codeActionIndex: 1);
    }

    [Fact]
    public async Task DecimalRoundValueReportsDiagnosticAndAddsToEven()
    {
        const string test = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return {|#0:decimal.Round(value)|};
                }
            }
            """;

        const string fixedCode = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return decimal.Round(value, MidpointRounding.ToEven);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Decimal"), fixedCode, codeActionIndex: 0);
    }

    [Fact]
    public async Task DecimalRoundValueReportsDiagnosticAndAddsAwayFromZero()
    {
        const string test = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return {|#0:decimal.Round(value)|};
                }
            }
            """;

        const string fixedCode = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return decimal.Round(value, MidpointRounding.AwayFromZero);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Decimal"), fixedCode, codeActionIndex: 1);
    }

    [Fact]
    public async Task DecimalRoundValueAndDigitsReportsDiagnosticAndAddsToEven()
    {
        const string test = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return {|#0:decimal.Round(value, 2)|};
                }
            }
            """;

        const string fixedCode = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return decimal.Round(value, 2, MidpointRounding.ToEven);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Decimal"), fixedCode, codeActionIndex: 0);
    }

    [Fact]
    public async Task DecimalRoundValueAndDigitsReportsDiagnosticAndAddsAwayFromZero()
    {
        const string test = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return {|#0:decimal.Round(value, 2)|};
                }
            }
            """;

        const string fixedCode = """
            using System;

            class C
            {
                decimal M(decimal value)
                {
                    return decimal.Round(value, 2, MidpointRounding.AwayFromZero);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("Decimal"), fixedCode, codeActionIndex: 1);
    }

    [Fact]
    public async Task MathFRoundValueReportsDiagnosticAndAddsToEven()
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

        const string fixedCode = """
            using System;

            class C
            {
                float M(float value)
                {
                    return MathF.Round(value, MidpointRounding.ToEven);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("MathF"), fixedCode, codeActionIndex: 0);
    }

    [Fact]
    public async Task MathFRoundValueReportsDiagnosticAndAddsAwayFromZero()
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

        const string fixedCode = """
            using System;

            class C
            {
                float M(float value)
                {
                    return MathF.Round(value, MidpointRounding.AwayFromZero);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("MathF"), fixedCode, codeActionIndex: 1);
    }

    [Fact]
    public async Task MathFRoundValueAndDigitsReportsDiagnosticAndAddsToEven()
    {
        const string test = """
            using System;

            class C
            {
                float M(float value)
                {
                    return {|#0:MathF.Round(value, 2)|};
                }
            }
            """;

        const string fixedCode = """
            using System;

            class C
            {
                float M(float value)
                {
                    return MathF.Round(value, 2, MidpointRounding.ToEven);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("MathF"), fixedCode, codeActionIndex: 0);
    }

    [Fact]
    public async Task MathFRoundValueAndDigitsReportsDiagnosticAndAddsAwayFromZero()
    {
        const string test = """
            using System;

            class C
            {
                float M(float value)
                {
                    return {|#0:MathF.Round(value, 2)|};
                }
            }
            """;

        const string fixedCode = """
            using System;

            class C
            {
                float M(float value)
                {
                    return MathF.Round(value, 2, MidpointRounding.AwayFromZero);
                }
            }
            """;

        await Verify.VerifyCodeFixAsync(test, Diagnostic().WithLocation(0).WithArguments("MathF"), fixedCode, codeActionIndex: 1);
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
                    var decimalToEven = Math.Round((decimal)value, MidpointRounding.ToEven);
                    var decimalAwayFromZero = Math.Round((decimal)value, 2, MidpointRounding.AwayFromZero);
                    var decimalStaticToEven = decimal.Round((decimal)value, MidpointRounding.ToEven);
                    var decimalStaticAwayFromZero = decimal.Round((decimal)value, 2, MidpointRounding.AwayFromZero);
                    var mathFToEven = MathF.Round((float)value, MidpointRounding.ToEven);
                    var mathFAwayFromZero = MathF.Round((float)value, 2, MidpointRounding.AwayFromZero);
                    return toEven + awayFromZero + (double)decimalToEven + (double)decimalAwayFromZero + (double)decimalStaticToEven + (double)decimalStaticAwayFromZero + mathFToEven + mathFAwayFromZero;
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

    [Fact]
    public async Task FullyQualifiedDecimalRoundReportsDiagnostic()
    {
        const string test = """
            class C
            {
                decimal M(decimal value)
                {
                    return {|#0:System.Decimal.Round(value, 2)|};
                }
            }
            """;

        await Verify.VerifyAnalyzerAsync(test, Diagnostic().WithLocation(0).WithArguments("Decimal"));
    }

    private static DiagnosticResult Diagnostic()
        => Verify.Diagnostic(ExplicitMidpointRoundingAnalyzer.DiagnosticId);
}
