using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = AnalyzerTemplate.Test.CSharpCodeFixVerifier<
    AnalyzerTemplate.ParseCasterAnalyzer,
    AnalyzerTemplate.CasterCodeFixProvider>;

namespace AnalyzerTemplate.Test
{
    [TestClass]
    public class ParseCasterTests
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
        
        [TestMethod]
        public async Task TestMethod2()
        {
            var test = @"
using System;
namespace TestApp
{
    public class Program
    {
        public static void Main()
        {
            var a = Convert.ToInt32( ""123"");
            var b = Convert.ToString(1);
        }
    }
}";

            var fixtest = @"
using System;
namespace TestApp
{
    public class Program
    {
        public static void Main()
        {
            var a = Int32.Parse(""123"");
            var b = Convert.ToString(1);
        }
    }
}";

            var expected = VerifyCS.Diagnostic("ParseCaster").WithSpan(9, 21, 9, 44);
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
