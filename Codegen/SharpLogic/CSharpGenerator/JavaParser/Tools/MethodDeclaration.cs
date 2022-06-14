using System.Collections.Generic;

namespace JavaParser.Tools
{
    public class MethodDeclaration
    {
        public string RequestType { get; set; }
        public string MethodName { get; set; }
        public string ReturnType { get; set; }
        public string Url { get; set; }
        public List<ArgDeclaration> ArgList = new();
    }
}