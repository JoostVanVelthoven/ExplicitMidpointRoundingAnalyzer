using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ExplicitMidpointRoundingAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExplicitMidpointRoundingAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "EMRA001";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Specify midpoint rounding explicitly",
        "Specify the MidpointRounding argument when calling {0}.Round",
        "Usage",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Math.Round defaults to MidpointRounding.ToEven; specify the rounding mode explicitly.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(RegisterCompilationActions);
    }

    private static void RegisterCompilationActions(CompilationStartAnalysisContext context)
    {
        var mathType = context.Compilation.GetTypeByMetadataName("System.Math");
        var mathFType = context.Compilation.GetTypeByMetadataName("System.MathF");
        var midpointRoundingType = context.Compilation.GetTypeByMetadataName("System.MidpointRounding");

        if (midpointRoundingType is null || (mathType is null && mathFType is null))
        {
            return;
        }

        context.RegisterOperationAction(
            operationContext => AnalyzeInvocation(operationContext, mathType, mathFType, midpointRoundingType),
            OperationKind.Invocation);
    }

    private static void AnalyzeInvocation(
        OperationAnalysisContext context,
        INamedTypeSymbol? mathType,
        INamedTypeSymbol? mathFType,
        INamedTypeSymbol midpointRoundingType)
    {
        var invocation = (IInvocationOperation)context.Operation;
        var method = invocation.TargetMethod;

        if (!IsSystemMathRound(method, mathType, mathFType) || HasMidpointRoundingParameter(method, midpointRoundingType))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.Syntax.GetLocation(), method.ContainingType.Name));
    }

    private static bool IsSystemMathRound(IMethodSymbol method, INamedTypeSymbol? mathType, INamedTypeSymbol? mathFType)
    {
        if (method.Name != "Round")
        {
            return false;
        }

        var containingType = method.ContainingType;
        return SymbolEqualityComparer.Default.Equals(containingType, mathType)
            || SymbolEqualityComparer.Default.Equals(containingType, mathFType);
    }

    private static bool HasMidpointRoundingParameter(IMethodSymbol method, INamedTypeSymbol midpointRoundingType)
    {
        return method.Parameters.Any(parameter => SymbolEqualityComparer.Default.Equals(parameter.Type, midpointRoundingType));
    }
}
