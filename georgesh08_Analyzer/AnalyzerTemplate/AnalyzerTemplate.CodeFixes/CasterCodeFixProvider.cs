using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AnalyzerTemplate
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CasterCodeFixProvider)), Shared]
    public class CasterCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray
            .Create(ParseCasterAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent
                .AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (declaration == null)
                return;
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CasterCodeFixTitle,
                    createChangedSolution: c => CastToParse(context.Document, declaration),
                    equivalenceKey: nameof(CodeFixResources.CasterCodeFixTitle)),
                diagnostic);
        }

        private async Task<Solution> CastToParse(Document doc, InvocationExpressionSyntax typeDecl)
        {
            var convertArg = typeDecl.ArgumentList.Arguments.First().Expression;
            var convertType = typeDecl.Expression.ChildNodes().OfType<IdentifierNameSyntax>().Last();
            var newType = convertType.ToString();
            if (newType.Substring(0, 2) != "To") return doc.Project.Solution;
            newType = newType.Substring(2);
            if (newType == "String" || newType == "Base64CharArray" || newType == "Base64String")
                return doc.Project.Solution;
            var args = ArgumentList(
                SingletonSeparatedList(
                    Argument(
                        convertArg)));
            var newExp = InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(newType),
                    IdentifierName("Parse"))).WithArgumentList(args);
            var newDoc = doc.WithSyntaxRoot((await doc.GetSyntaxRootAsync())
                .ReplaceNode(typeDecl, newExp).WithAdditionalAnnotations());
            return newDoc.Project.Solution;
        }
    }
}
