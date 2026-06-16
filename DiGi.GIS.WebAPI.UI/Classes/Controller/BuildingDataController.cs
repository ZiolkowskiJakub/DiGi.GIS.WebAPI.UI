using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    [Route("[controller]")]
    public class BuildingDataController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public BuildingDataController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet("tablebyreference")]
        public async Task<IActionResult> GetTableByReferenceAsync([FromQuery(Name = "reference")] string reference, [FromQuery(Name = "countyid")] int? countyId = null)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/buildingdata/tablebyreference");
            urlBuilder = urlBuilder.AddParameter("reference", reference);

            if(countyId is not null)
            {
                urlBuilder = urlBuilder.AddParameter("countyid", countyId.Value);
            }


            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            string json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            // Here we use your DLL to turn JSON back into real C# objects.
            // Note: Since AdministrativeAreal2D is abstract,
            // you might need a specific converter or a concrete type.
            DiGi.PostgreSQL.Table.Classes.Table? table = Core.Convert.ToDiGi<DiGi.PostgreSQL.Table.Classes.Table>(json)?.FirstOrDefault();
            if(table is null)
            {
                return BadRequest();
            }

            // We pass the objects to a Partial View
            return PartialView("_TableView", new TableView(table));
        }

        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }
    }
}