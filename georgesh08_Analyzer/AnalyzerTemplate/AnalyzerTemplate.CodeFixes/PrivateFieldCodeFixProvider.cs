using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace AnalyzerTemplate
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PrivateFieldCodeFixProvider)), Shared]
    public class PrivateFieldCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray
            .Create(PrivateFieldAnalyzer.DiagnosticId);

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
                .AncestorsAndSelf().OfType<FieldDeclarationSyntax>().FirstOrDefault();
            if (declaration == null)
                return;
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.PrivateFieldCodeFixTitle,
                    createChangedDocument: c => RemoveField(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.PrivateFieldCodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> RemoveField(Document doc, FieldDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        {
            
            var variableName = typeDecl.Declaration.Variables.First().Identifier.ToString();
            var variableType = typeDecl.Declaration.Type;
            var root = await doc.GetSyntaxRootAsync(cancellationToken);
            root = root.RemoveNode(typeDecl, SyntaxRemoveOptions.KeepNoTrivia);
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            
            foreach (var method in methods)
            {
                var expressions = method.Body.DescendantNodes()
                    .OfType<ExpressionStatementSyntax>();
                foreach (var expression in expressions)
                {
                    if (!expression.Expression.IsKind(SyntaxKind.SimpleAssignmentExpression)) continue;
                    if (!expression.Expression.ChildNodes().First().WithoutTrivia().ToString()
                            .Equals(variableName)) continue;
                    var declaration = LocalDeclarationStatement(
                        VariableDeclaration(
                                IdentifierName(
                                    variableType.ToString()))
                            .WithVariables(
                                SingletonSeparatedList(
                                    VariableDeclarator(
                                        variableName))));


                    var newRoot = root;
                    newRoot = newRoot.InsertNodesBefore(expression, new List<SyntaxNode>() {declaration});
                    root = newRoot;
                    break;
                }
            }
            return doc.WithSyntaxRoot(root);
        }
    }
}
