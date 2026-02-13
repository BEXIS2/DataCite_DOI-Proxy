using Newtonsoft.Json;
using Vaelastrasz.Library.Configurations;
using Vaelastrasz.Library.Extensions;
using Vaelastrasz.Library.Models;
using Vaelastrasz.Library.Services;
using Vaelastrasz.Library.Settings;

namespace Vaelastrasz.Library.Tests.Services
{
    public class DataCiteServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            //var config = new Configuration("Test_DOIProxy_User", "s~c9<evQ#%^h4Uyb", "https://taerar.infinite-trajectory.de", new List<string>());
            var config = new Configuration("sventhiel", "sventhiel", "https://taerar.infinite-trajectory.de");

            var dataCiteService = new DataCiteService(config);

            var x = await dataCiteService.GetAsync();
        }

        [Test]
        public async Task Test2()
        {
            try
            {
                var config = new Configuration("sventhiel", "sventhiel", "https://taerar.infinite-trajectory.de");

                string text = File.ReadAllText(@"C:/Projects/github.com/sventhiel/Vaelastrasz/Vaelastrasz.Server/Examples/doi_002.json");
                var data = JsonConvert.DeserializeObject<CreateDataCiteModel>(text);

                var dataCiteService = new DataCiteService(config);

                var x = await dataCiteService.CreateAsync(data);
                var y = await dataCiteService.GetCitationStyleByDoiAsync("10.23720%2Fapitest005", Types.DataCiteCitationStyleType.APA);
                var z = await dataCiteService.GetMetadataFormatByDoiAsync("10.23720%2Fapitest005", Types.DataCiteMetadataFormatType.BibTeX);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR", ex.ToString());
            }
        }

        [Test]
        public void Test3()
        {
            JsonConvert.DefaultSettings = () => VaelastraszJsonSerializerSettings.Settings;

            //string text = File.ReadAllText(@"C:/Projects/github.com/sventhiel/Vaelastrasz/Vaelastrasz.Server/Examples/creator_001.json");
            //var x = JsonConvert.DeserializeObject<DataCiteCreator>(text);

            var model = new CreateDataCiteModel();
            model.SetType(Types.DataCiteType.DOIs)
                .SetTypes(Types.DataCiteResourceTypeGeneral.Dataset, "", "", "", "", "")
                .AddCreator("Sven Thiel", Types.DataCiteNameType.Personal)
                .AddTitle("Test", "English", Types.DataCiteTitleType.Subtitle)
                .SetDoi("10.25829/45b58z45")
                .SetEvent(Types.DataCiteEventType.Hide)
                .SetPublicationYear(2024)
                .SetPublisher("test", "resd", "sdfsdf", "sdfsdf", "English")
                .AddDescription(new Library.Models.DataCite.DataCiteDescription() { Description = "This dataset contains results from the continuous monitoring program of high-precision measurements of mole fracions of greenhouse gases,stable isotopes in CO2 and CH4, and radiocarbon in CO2, obtained at the Amazon Tall Tower Observatory (ATTO), located in the central Amazon region of Brazil. In September 2021, we installed an automated sampler designed and built by the Integrated Carbon Observation System (ICOS) to collect air samples in 3 litter flasks at a height of 324 m above the ground level. Samples are collected weekly, during an one-hour integration time between 13:00 and 14:00 h local time (17:00-18:00 UTC). The flasks are shipped to Jena, Germany, for analyses of CO2, CO, CH4, N2O, H2, SF6, 13C-CO2, 14C-CO2, 18O-CO2, 13C-CH4, 2H-CH4, O2/N2, and Ar/N2 at the laboratories of the Max Planck Institute for Biogeochemistry (MPI-BGC). Measurements from the flask system provide reference information for this site and act as an independent quality control for the other high frequency measurements. This dataset covers the period from 2021-09-09 to 2024-05-23.", DescriptionType = Types.DataCiteDescriptionType.Abstract })
                .SetUrl("https://google.de");

            //model = model.Update(new List<string>());

            var x = JsonConvert.SerializeObject(model);

            //var config = new Configuration("Test_DOIProxy_User", "s~c9<evQ#%^h4Uyb", "https://taerar.infinite-trajectory.de", true);
            //var config = new Configuration("biodivbank2", "biodivbank2", "https://doi.bexis2.uni-jena.de", true);

            //var dataCiteService = new DataCiteService(config);

            //var response = await dataCiteService.CreateAsync(model);
        }

        [Test]
        public void Test4()
        {
            var config = new Configuration("sventhiel", "proq3dm6", "http://localhost:5041");
            var dataCiteService = new DataCiteService(config);
            var doiService = new DOIService(config);

            var model = new CreateDataCiteModel();

            model.Data.Attributes.Doi = "";

            //var x = await dataCiteService.CreateAsync(data);
        }
    }
}