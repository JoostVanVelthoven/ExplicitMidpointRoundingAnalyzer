using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;

namespace ExplicitMidpointRoundingAnalyzer;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExplicitMidpointRoundingCodeFixProvider))]
[Shared]
public sealed class ExplicitMidpointRoundingCodeFixProvider : CodeFixProvider
{
    private const string ToEvenTitle = "Specify MidpointRounding.ToEven";
    private const string AwayFromZeroTitle = "Specify MidpointRounding.AwayFromZero";

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(ExplicitMidpointRoundingAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics[0];
        var invocation = FindInvocation(root, diagnostic.Location.SourceSpan);

        if (invocation is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CreateCodeAction(context.Document, invocation, ToEvenTitle, "ToEven"),
            diagnostic);

        context.RegisterCodeFix(
            CreateCodeAction(context.Document, invocation, AwayFromZeroTitle, "AwayFromZero"),
            diagnostic);
    }

    private static CodeAction CreateCodeAction(
        Document document,
        InvocationExpressionSyntax invocation,
        string title,
        string roundingModeName)
    {
        return CodeAction.Create(
            title,
            cancellationToken => AddMidpointRoundingArgumentAsync(document, invocation, roundingModeName, cancellationToken),
            equivalenceKey: title);
    }

    private static InvocationExpressionSyntax? FindInvocation(SyntaxNode? root, TextSpan sourceSpan)
    {
        return root?.FindNode(sourceSpan, getInnermostNodeForTie: true)
            .FirstAncestorOrSelf<InvocationExpressionSyntax>();
    }

    private static async Task<Document> AddMidpointRoundingArgumentAsync(
        Document document,
        InvocationExpressionSyntax invocation,
        string roundingModeName,
        CancellationToken cancellationToken)
    {
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (semanticModel is null || root is null)
        {
            return document;
        }

        var midpointRoundingTypeName = GetMidpointRoundingTypeName(semanticModel, invocation);
        var argument = SyntaxFactory.Argument(SyntaxFactory.ParseExpression($"{midpointRoundingTypeName}.{roundingModeName}"))
            .WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);
        var newArgumentList = invocation.ArgumentList.AddArguments(argument);
        var newInvocation = invocation.WithArgumentList(newArgumentList);
        var newRoot = root.ReplaceNode(invocation, newInvocation);

        return document.WithSyntaxRoot(newRoot);
    }

    private static string GetMidpointRoundingTypeName(SemanticModel semanticModel, InvocationExpressionSyntax invocation)
    {
        var midpointRoundingType = semanticModel.Compilation.GetTypeByMetadataName("System.MidpointRounding");
        return midpointRoundingType?.ToMinimalDisplayString(semanticModel, invocation.SpanStart) ?? "System.MidpointRounding";
    }
}
