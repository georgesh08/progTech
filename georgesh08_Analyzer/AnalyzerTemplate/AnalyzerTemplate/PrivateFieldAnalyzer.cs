using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AnalyzerTemplate
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PrivateFieldAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PrivateFixer";
        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.PrivateFieldAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.PrivateFieldMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.PrivateFieldAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Fix";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, 
            isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.FieldDeclaration);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var field = (FieldDeclarationSyntax) context.Node;
            if (!field.ChildTokens().First().IsKind(SyntaxKind.PrivateKeyword)) return;
            var variableName = field.Declaration.Variables.First().Identifier.ToString();
            var methods = field.Parent.ChildNodes().OfType<MethodDeclarationSyntax>();
            var enableWarning = true;
            foreach (var method in methods)
            {
                var statements = method.Body.ChildNodes();
                foreach (var statement in statements)
                {
                    if (statement.IsKind(SyntaxKind.ExpressionStatement))
                    {
                        var expressionType = statement.ChildNodes().First();
                        if (expressionType.IsKind(SyntaxKind.InvocationExpression))
                        {
                            var arg = statement.ChildNodes().OfType<ArgumentListSyntax>()
                                .First().Arguments.First();
                            if (arg.ChildNodes().OfType<IdentifierNameSyntax>().First().WithoutTrivia().ToString()
                                .Equals(variableName))
                            {
                                enableWarning = false;
                            }
                        }

                        if (expressionType.IsKind(SyntaxKind.SimpleAssignmentExpression))
                        {
                            var operand = expressionType.ChildNodes().Last();
                            if (operand.WithoutTrivia().ToString().Equals(variableName))
                            {
                                enableWarning = false;
                            }
                        }
                    }

                    if (!statement.IsKind(SyntaxKind.LocalDeclarationStatement)) continue;
                    var variableDeclarationSyntax = statement.ChildNodes()
                        .OfType<VariableDeclarationSyntax>().First();
                    var initializer = variableDeclarationSyntax.Variables.FirstOrDefault();
                    if (initializer == null)
                    {
                        return;
                    }

                    var value = initializer.Initializer.Value;
                    if (value.WithoutTrivia().ToString().Equals(variableName))
                    {
                        enableWarning = false;
                    }
                }

                if (!enableWarning)
                {
                    break;
                }
            }

            if (enableWarning)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, field.GetLocation()));
            }
        }
    }
}
