using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.PostgreSQL.Classes;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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

        [HttpGet("tablebyid")]
        public async Task<IActionResult> GetTableByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/building2D/building2Dreferencesbyadministrativeareal2Did");
            //urlBuilder = urlBuilder.AddParameter("administrativeareal2Did", administrativeAreal2DId);

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
            List<Building2DReference>? building2DReferences = Core.Convert.ToDiGi<Building2DReference>(json);

            // We pass the objects to a Partial View
            return PartialView("_BuildingDataView", new Building2DReferencesView(building2DReferences));
        }

        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }
    }
}