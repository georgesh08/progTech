using System.IO;
using JavaParser.ParserFolder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MyGenerator
{
    public class DtoGenerator
    {
        private Parser _parser;

        public DtoGenerator(Parser parser)
        {
            _parser = parser;
        }

        public void Generate()
        {
            _parser.Parse();
            foreach (var constructor in _parser.DtoConstructors())
            {
                var tree = CompilationUnit();
                if (constructor.ClassName == "Owner")
                {
                    tree = tree.WithUsings(SingletonList(
                        UsingDirective(
                            QualifiedName(
                                QualifiedName(
                                    IdentifierName("System"),
                                    IdentifierName("Collections")),
                                IdentifierName("Generic")))));
                }
                var @namespace = NamespaceDeclaration(ParseName("MyGenerator"));
                var classDeclaration = ClassDeclaration(constructor.ClassName)
                    .AddModifiers(
                        Token(
                            SyntaxKind.PublicKeyword));
                MemberDeclarationSyntax[] members = new MemberDeclarationSyntax[constructor.Fields().Count];
                var counter = 0;
                foreach (var field in constructor.Fields())
                {
                    var variableDeclaration = VariableDeclaration(ParseTypeName(field.Type))
                        .AddVariables(VariableDeclarator(field.Name));
                    var fieldDeclaration = FieldDeclaration(variableDeclaration)
                        .AddModifiers(Token(SyntaxKind.PublicKeyword));
                    members[counter] = fieldDeclaration;
                    counter++;
                }

                classDeclaration = classDeclaration.AddMembers(members);
                @namespace = @namespace.AddMembers(classDeclaration);
                tree = tree.AddMembers(@namespace);
                string filePath = @"C:\IsTech-y24\Codegen\SharpLogic\CSharpGenerator\MyGenerator\" + constructor.ClassName + ".cs";
                File.WriteAllText(filePath, tree.NormalizeWhitespace().ToString());
            }
        }
    }
}