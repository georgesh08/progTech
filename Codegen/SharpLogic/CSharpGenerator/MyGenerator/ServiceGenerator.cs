using System.Collections.Generic;
using System.IO;
using JavaParser.ParserFolder;
using JavaParser.Tools;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MyGenerator
{
    public class ServiceGenerator
    {
        private readonly Parser _parser;
        private readonly string _serverUrl;

        public ServiceGenerator(Parser parser, string server)
        {
            _parser = parser;
            _serverUrl = server;
        }

        public void Generate()
        {
            var tree = CompilationUnit();
            
            tree = SetUsings(tree);

            var classDeclaration = BuildClass();

            var @namespace = BuildNamespace();

            var clientFieldDeclaration = BuildHttpClient();

            MemberDeclarationSyntax[] methods = new MemberDeclarationSyntax[_parser.MethodsDeclaration().Count + 1];
            methods[0] = clientFieldDeclaration;
            var i = 1;
            foreach (var method in _parser.MethodsDeclaration())
            {
                BlockSyntax block = method.RequestType == "Get" ? 
                    GenerateGetMethodBlock(method, _serverUrl) 
                    : GeneratePostMethodBlock(method, _serverUrl);
                var args = GenerateParams(method.ArgList);
                methods[i] = MethodDeclaration(
                        GenericName(
                                Identifier("Task"))
                            .WithTypeArgumentList(
                                TypeArgumentList(
                                    SingletonSeparatedList<TypeSyntax>(
                                        IdentifierName("string")))),
                        Identifier(method.MethodName))
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword)))
                    .WithParameterList(args)
                    .WithBody(block);
                i++;
            }

            classDeclaration = classDeclaration.AddMembers(methods);
            @namespace = @namespace.AddMembers(classDeclaration);
            tree = tree.AddMembers(@namespace);

            string filePath = @"C:\IsTech-y24\Codegen\SharpLogic\CSharpGenerator\MyGenerator\" + "Service.cs";
            File.WriteAllText(filePath, tree.NormalizeWhitespace().ToString());
        }
        private CompilationUnitSyntax SetUsings(CompilationUnitSyntax tree)
        {
            return tree.WithUsings(
                List(
                    new[]
                    {
                        UsingDirective(
                            QualifiedName(
                                QualifiedName(
                                    IdentifierName("System"),
                                    IdentifierName("Net")),
                                IdentifierName("Http"))),
                        UsingDirective(
                            QualifiedName(
                                IdentifierName("System"),
                                IdentifierName("Text"))),
                        UsingDirective(
                            QualifiedName(
                                QualifiedName(
                                    IdentifierName("System"),
                                    IdentifierName("Threading")),
                                IdentifierName("Tasks"))),
                        UsingDirective(
                            QualifiedName(
                                IdentifierName("Newtonsoft"),
                                IdentifierName("Json")))
                    }));        
        }

        private ClassDeclarationSyntax BuildClass()
        {
            return ClassDeclaration("Service")
                .AddModifiers(
                    Token(
                        SyntaxKind.PublicKeyword));
        }

        private NamespaceDeclarationSyntax BuildNamespace()
        {
            return NamespaceDeclaration(ParseName("MyGenerator"));
        }

        private AwaitExpressionSyntax PostMethodsReturnStatement()
        {
            return AwaitExpression(
                InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("response"),
                            IdentifierName("Content")),
                        IdentifierName("ReadAsStringAsync"))));
        }

        private FieldDeclarationSyntax BuildHttpClient()
        {
            var clientVariableDeclaration = VariableDeclaration(
                    ParseTypeName("HttpClient"))
                .AddVariables(
                    VariableDeclarator("client")
                    .WithInitializer(
                        EqualsValueClause(
                            ObjectCreationExpression(
                                    IdentifierName("HttpClient"))
                                .WithArgumentList(ArgumentList()))));
            return FieldDeclaration(clientVariableDeclaration)
                .AddModifiers(
                    Token(
                    SyntaxKind.PrivateKeyword));
        }

        private BlockSyntax GenerateGetMethodBlock(MethodDeclaration method, string serverPath)
        {
            var queryString = CreateQueryString(method, serverPath);
            return Block(
                SingletonList<StatementSyntax>(
                    ReturnStatement(
                        AwaitExpression(
                            InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression, 
                                        IdentifierName("client"), 
                                        IdentifierName("GetStringAsync")))
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(
                                                InterpolatedStringExpression(
                                                        Token(SyntaxKind.InterpolatedStringStartToken))
                                                    .WithContents(
                                                        List(
                                                            new InterpolatedStringContentSyntax[]{
                                                                InterpolatedStringText()
                                                                    .WithTextToken(
                                                                        Token(
                                                                            TriviaList(), 
                                                                            SyntaxKind.InterpolatedStringTextToken, 
                                                                            queryString,
                                                                            queryString,
                                                                            TriviaList()))}))))))))));
        }

        private BlockSyntax GeneratePostMethodBlock(MethodDeclaration method, string serverPath)
        {
            var queryString = CreateQueryString(method, serverPath);
            var returnStatement = ReturnStatement(PostMethodsReturnStatement());
            var jsonString = GenerateJsonString(method);
            var content = GeneratePostMethodContent();
            var response = GenerateResponse(queryString);
            return Block(jsonString, content, response, returnStatement);
        }

        private LocalDeclarationStatementSyntax GenerateResponse(string queryString)
        {
            return LocalDeclarationStatement(
                VariableDeclaration(
                        IdentifierName(
                            Identifier(
                                TriviaList(),
                                SyntaxKind.VarKeyword,
                                "var",
                                "var",
                                TriviaList())))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                    Identifier("response"))
                                .WithInitializer(
                                    EqualsValueClause(
                                        AwaitExpression(
                                            InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("client"),
                                                        IdentifierName("PostAsync")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                Argument(
                                                                    LiteralExpression(
                                                                        SyntaxKind.StringLiteralExpression,
                                                                        Literal(queryString))),
                                                                Token(SyntaxKind.CommaToken),
                                                                Argument(
                                                                    IdentifierName("content"))
                                                            })))))))));
        }
        private LocalDeclarationStatementSyntax GeneratePostMethodContent()
        {
            return LocalDeclarationStatement(
                VariableDeclaration(
                        IdentifierName(
                            Identifier(
                                TriviaList(),
                                SyntaxKind.VarKeyword,
                                "var",
                                "var",
                                TriviaList())))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                    Identifier("content"))
                                .WithInitializer(
                                    EqualsValueClause(
                                        ObjectCreationExpression(
                                                IdentifierName("StringContent"))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SeparatedList<ArgumentSyntax>(
                                                        new SyntaxNodeOrToken[]
                                                        {
                                                            Argument(
                                                                IdentifierName("json")),
                                                            Token(SyntaxKind.CommaToken),
                                                            Argument(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    IdentifierName("Encoding"),
                                                                    IdentifierName("UTF8"))),
                                                            Token(SyntaxKind.CommaToken),
                                                            Argument(
                                                                LiteralExpression(
                                                                    SyntaxKind.StringLiteralExpression,
                                                                    Literal("application/json")))
                                                        }))))))));
        }
        private LocalDeclarationStatementSyntax GenerateJsonString(MethodDeclaration method)
        {
            return LocalDeclarationStatement(
                VariableDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.StringKeyword)))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                    Identifier("json"))
                                .WithInitializer(
                                    EqualsValueClause(
                                        InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("JsonConvert"),
                                                    IdentifierName("SerializeObject")))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList(
                                                        Argument(
                                                            IdentifierName(method.ArgList[0].Name))))))))));
        }
        
        private string CreateQueryString(MethodDeclaration method, string serverPath)
        {
            if (method.RequestType == "Post") return serverPath + method.Url + "/";
            string queryString = serverPath + method.Url + "/?";
            var counter = 0;
            foreach (var arg in method.ArgList)
            {
                var resString = arg.Name + "=" + "{" + arg.Name + "}";
                if(counter != 0)
                {
                    queryString += "&" + resString;
                }
                else
                {
                    queryString += resString;
                }

                counter++;
            }

            return queryString;
        }
        
        private ParameterListSyntax GenerateParams(List<ArgDeclaration> args)
        {
            SyntaxNodeOrToken[] nodes = new SyntaxNodeOrToken[2*args.Count - 1];
            for (int i = 0; i < args.Count; ++i)
            {
                if (i == args.Count - 1)
                {
                    nodes[i] = Parameter(
                            Identifier(args[i].Name))
                        .WithType(
                            IdentifierName(args[i].Type));
                }
                else
                {
                    nodes[i] = Parameter(
                            Identifier(args[i].Name))
                        .WithType(
                            IdentifierName(args[i].Type));
                    nodes[i + 1] = Token(SyntaxKind.CommaToken);
                }
            }

            return ParameterList(
                SeparatedList<ParameterSyntax>(nodes));
        }
    }
}