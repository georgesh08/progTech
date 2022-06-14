using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = AnalyzerTemplate.Test.CSharpCodeFixVerifier<
    AnalyzerTemplate.PrivateFieldAnalyzer,
    AnalyzerTemplate.PrivateFieldCodeFixProvider>;

namespace AnalyzerTemplate.Test
{
    [TestClass]
    public class PrivateFieldTests
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}