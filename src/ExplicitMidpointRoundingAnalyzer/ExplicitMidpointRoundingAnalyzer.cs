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
        description: "Round overloads without a MidpointRounding argument default to MidpointRounding.ToEven; specify the rounding mode explicitly.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(RegisterCompilationActions);
    }

    private static void RegisterCompilationActions(CompilationStartAnalysisContext context)
    {
        var roundTypes = new[]
        {
            context.Compilation.GetTypeByMetadataName("System.Math"),
            context.Compilation.GetTypeByMetadataName("System.MathF"),
            context.Compilation.GetTypeByMetadataName("System.Decimal"),
            context.Compilation.GetTypeByMetadataName("System.Half"),
            context.Compilation.GetTypeByMetadataName("System.Double"),
            context.Compilation.GetTypeByMetadataName("System.Single"),
            context.Compilation.GetTypeByMetadataName("System.Numerics.IFloatingPoint`1"),
        }
        .Where(type => type is not null)
        .Cast<INamedTypeSymbol>()
        .ToImmutableArray();
        var midpointRoundingType = context.Compilation.GetTypeByMetadataName("System.MidpointRounding");

        if (midpointRoundingType is null || roundTypes.IsEmpty)
        {
            return;
        }

        context.RegisterOperationAction(
            operationContext => AnalyzeInvocation(operationContext, roundTypes, midpointRoundingType),
            OperationKind.Invocation);
    }

    private static void AnalyzeInvocation(
        OperationAnalysisContext context,
        ImmutableArray<INamedTypeSymbol> roundTypes,
        INamedTypeSymbol midpointRoundingType)
    {
        var invocation = (IInvocationOperation)context.Operation;
        var method = invocation.TargetMethod;

        if (!IsSystemRound(method, roundTypes) || HasMidpointRoundingParameter(method, midpointRoundingType))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.Syntax.GetLocation(), method.ContainingType.Name));
    }

    private static bool IsSystemRound(IMethodSymbol method, ImmutableArray<INamedTypeSymbol> roundTypes)
    {
        if (method.Name != "Round")
        {
            return false;
        }

        var containingType = method.ContainingType;
        return roundTypes.Any(roundType =>
            SymbolEqualityComparer.Default.Equals(containingType, roundType) ||
            SymbolEqualityComparer.Default.Equals(containingType.OriginalDefinition, roundType));
    }

    private static bool HasMidpointRoundingParameter(IMethodSymbol method, INamedTypeSymbol midpointRoundingType)
    {
        return method.Parameters.Any(parameter => SymbolEqualityComparer.Default.Equals(parameter.Type, midpointRoundingType));
    }
}
