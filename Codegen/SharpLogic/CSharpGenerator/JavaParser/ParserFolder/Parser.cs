using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using JavaParser.Tools;

namespace JavaParser.ParserFolder
{
    public class Parser
    {
        public string SourcePath;
        private List<MethodDeclaration> _methods = new List<MethodDeclaration>();
        private List<DtoConstructor> _dto = new List<DtoConstructor>();

        public Parser(string sourcePath)
        {
            SourcePath = sourcePath;
        }

        public List<MethodDeclaration> MethodsDeclaration()
        {
            return _methods;
        }

        public List<DtoConstructor> DtoConstructors()
        {
            return _dto;
        }

        public void Parse()
        {
            string[] files = GetSourceFilesPath();
            foreach (string path in files)
            {
                string sourceCode = File.ReadAllText(path);
                Regex controllerMarker = new("@RestController");
                Regex databaseMarker = new("Database");
                Regex mainMethodMarker = new("main");
                if(databaseMarker.IsMatch(sourceCode) || mainMethodMarker.IsMatch(sourceCode)) continue;
                if (controllerMarker.IsMatch(sourceCode))
                {
                    ControllerParser(sourceCode);
                }
                else
                {
                    PojoParser(sourceCode);
                }
            }
        }

        private void ControllerParser(string sourceCode)
        {
            var methodSearchRgx = new Regex("@GetMapping|@PostMapping");

            var counter = 1;
            
            foreach (Match match in methodSearchRgx.Matches(sourceCode))
            {
                MethodDeclaration declaration = new();
                SetMethodUrl(sourceCode, declaration);
                SetRequestType(match, declaration);
                SetMethodNameAndReturnType(sourceCode, declaration, counter);
                SetArguments(sourceCode, counter - 1, declaration);
                _methods.Add(declaration);
                counter++;
            }
        }

        private void PojoParser(string sourceCode)
        {
            DtoConstructor constructor = new DtoConstructor();
            var classNameRgx = new Regex("class \\w{1,}");
            constructor.ClassName = classNameRgx.Match(sourceCode).Value.Split(' ')[1];
            var fieldsRgx = new Regex(@"(\w{1,} \w{1,};)|([\S]* \w{1,} =)");
            foreach (Match match in fieldsRgx.Matches(sourceCode))
            {
                Field field = new Field();
                var str = match.Value.Replace(";", "").Split(' ');
                var type = str[0];
                var name = str[1];
                if (type.Contains("Integer"))
                {
                    type = type.Replace("Integer", "int");
                }
                else if (type.Contains("String"))
                {
                    type = type.ToLower();
                }

                field.Type = type;
                field.Name = name;
                constructor.AddField(field);
            }
            _dto.Add(constructor);
         }

        private void SetArguments(string sourcecode, int counter, MethodDeclaration declaration)
        {
            var rgx = new Regex(@"\(@Request\w{1,} .*\)");
            var match = rgx.Matches(sourcecode)[counter];
            var prms = match.Value.Replace(",", "").Split(' ');
            for (int i = 0; i < prms.Length - 1; ++i)
            {
                ArgDeclaration arg = new ArgDeclaration();
                if (!prms[i].Contains('@'))
                {
                    arg.Type = prms[i];
                    arg.Name = prms[i + 1].Replace(")", "");
                    if (arg.Type == "String") arg.Type = arg.Type.ToLower();
                    declaration.ArgList.Add(arg);
                }
            }
        }
        
        private void SetMethodNameAndReturnType(string sourceCode, MethodDeclaration declaration, int counter)
        {
            var rgx = new Regex("public \\w{1,} \\w{1,}");
            var match = rgx.Matches(sourceCode)[counter];
            var tmp = match.Value.Split(' ');
            switch (tmp[1])
            {
                case "class":
                    return;
                case ("String"):
                    declaration.ReturnType = tmp[1].ToLower();
                    declaration.MethodName = tmp[2];
                    break;
                default:
                    declaration.ReturnType = tmp[1];
                    declaration.MethodName = tmp[2];
                    break;
            }
        }

        private void SetMethodUrl(string sourceCode, MethodDeclaration declaration)
        {
            var urlRgx = new Regex("\"\\w{1,}\"");
            var tmp = urlRgx.Match(sourceCode);
            declaration.Url = tmp.Value.Replace("\"", "");
        }

        private void SetRequestType(Match match, MethodDeclaration declaration)
        {
            var requestTypeRgx = new Regex("@\\w{1,}M");
            var requestType = requestTypeRgx.Match(match.Value).Value.Replace("@", "");
            declaration.RequestType = requestType.Replace("M", "");
        }

        private string[] GetSourceFilesPath()
        {
            return Directory.GetFiles(SourcePath, "*.java", SearchOption.AllDirectories);
        }
    }
}