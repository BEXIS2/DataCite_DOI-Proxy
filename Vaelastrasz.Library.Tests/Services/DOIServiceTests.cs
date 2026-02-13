using Vaelastrasz.Library.Configurations;
using Vaelastrasz.Library.Models;
using Vaelastrasz.Library.Services;

namespace Vaelastrasz.Library.Tests.Services
{
    public class DOIServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            try
            {
                //var config = new Configuration("Test_DOIProxy_User", "s~c9<evQ#%^h4Uyb", "https://taerar.infinite-trajectory.de");
                //var config = new Configuration("Test_DOIProxy_User", "sventhiel", "http://localhost:5041");
                var config = new Configuration("sventhiel", "sventhiel", "https://taerar.infinite-trajectory.de");
                //var config = new Configuration("sventhiel", "s~c9<evQ#%^h4Uyb", "https://doi-proxy.bgc-jena.mpg.de");

                var doiService = new DOIService(config);

                var x = await doiService.GetAsync();

                //var dict = "{ \"placeholders\": { \"{DatasetId}\": \"340\", \"{VersionId}\": \"1408\", \"{VersionNumber}\": \"2\" } }";

                var dict = new Dictionary<string, string>
                {
                    { "{DatasetId}", "1" },
                    {"{VersionId}", "asdasd" },
                    {"{VersionNumbers}", "111" }
                };

                var createSuffixModel = new CreateSuffixModel()
                {
                    Placeholders = dict
                };

                var doi = await doiService.GenerateAsync(createSuffixModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}