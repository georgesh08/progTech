using JavaParser.ParserFolder;

namespace MyGenerator
{
    public class Program
    {
        public static void Main()
        {
            Parser parser = new
                ("C:\\IsTech-y24\\Codegen\\JavaServer\\service\\src\\main\\java\\tech\\is\\service");
            DtoGenerator gen = new(parser);
            gen.Generate();
            ServiceGenerator serviceGen = new(parser, "http://localhost:8080/");
            serviceGen.Generate();
        }
    }
}