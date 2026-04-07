using DiGi.GIS.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    [Route("[controller]")]
    public class AdministrativeAreal2DController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        // Constructor injection for the PostgreSQL data source
        public AdministrativeAreal2DController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        // This action will trigger for: gis.digiproject.uk/administrativeareal2D
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }

        [HttpGet("itemsbyadministrativearealtype")]
        public async Task<IActionResult> GetItemsByAdministrativeArealTypeAsync([FromQuery(Name = "administrativearealtype")] string administrativeArealType)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();
            string url = $"https://api.digiproject.uk/gis/administrativeareal2D/itemsbyadministrativearealtype?administrativearealtype={administrativeArealType}";

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);
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
            List<AdministrativeAreal2D>? administrativeAreal2Ds = Core.Convert.ToDiGi<AdministrativeAreal2D>(json);

            // We pass the objects to a Partial View
            return PartialView("_AdministrativeAreal2Ds", administrativeAreal2Ds ?? []);
        }
    }
}
