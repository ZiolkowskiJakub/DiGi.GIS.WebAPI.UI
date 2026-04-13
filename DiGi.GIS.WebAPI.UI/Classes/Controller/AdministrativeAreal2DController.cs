using DiGi.Core.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using DiGi.GIS.PostgreSQL;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet("administrativeareal2Dreferencesbyadministrativearealtype")]
        public async Task<IActionResult> GetAdministrativeAreal2DReferencesByAdministrativeArealTypeAsync([FromQuery(Name = "administrativearealtype")] string administrativeArealType, [FromQuery(Name = "parentid")] int? parentId, [FromQuery(Name = "uniquecode")] bool? uniqueCode)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencesbyadministrativearealtype");
            if (!string.IsNullOrWhiteSpace(administrativeArealType))
            {
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", administrativeArealType);
            }

            if (parentId is not null && parentId.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("parentId", parentId.Value);
            }

            if (uniqueCode is not null && uniqueCode.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("uniquecode", uniqueCode.Value);
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
            List<PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReference>(json);

            // We pass the objects to a Partial View
            return PartialView("_AdministrativeAreal2DReferences", administrativeAreal2DReferences ?? []);
        }

        [HttpGet("administrativeareal2Dreferencesbycode")]
        public async Task<IActionResult> GetAdministrativeAreal2DReferencesByCodeAsync([FromQuery(Name = "code")] string code)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region AdministrativeAreal2DReference

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencebycode");
            if (code is not null)
            {
                urlBuilder = urlBuilder.AddParameter("code", code);
            }

            httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            PostgreSQL.Classes.AdministrativeAreal2DReference? administrativeAreal2DReference = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReference>(json)?.FirstOrDefault();
            if (administrativeAreal2DReference is null)
            {
                return NotFound();
            }

            if (administrativeAreal2DReference.AdministrativeArealType == PostgreSQL.Enums.AdministrativeArealType.Subdivison && administrativeAreal2DReference.AdministrativeArealType.ParentAdministrativeArealType() is PostgreSQL.Enums.AdministrativeArealType administrativeArealType_Parent)
            {
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", administrativeArealType_Parent.ToString());

                httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    return BadRequest();
                }

                json = await httpResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json))
                {
                    return NoContent();
                }

                administrativeAreal2DReference = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReference>(json)?.FirstOrDefault();
                if (administrativeAreal2DReference is null)
                {
                    return NotFound();
                }
            }

            #endregion AdministrativeAreal2DReference

            #region AdministrativeAreal2D

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/itembyid");
            if (code is not null)
            {
                urlBuilder = urlBuilder.AddParameter("id", administrativeAreal2DReference.Id);
            }

            httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            AdministrativeAreal2D? administrativeAreal2D = Core.Convert.ToDiGi<AdministrativeAreal2D>(json)?.FirstOrDefault();
            if (administrativeAreal2D is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2D

            #region AdministrativeAreal2DReferencePath

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencepathbyid");
            if (code is not null)
            {
                urlBuilder = urlBuilder.AddParameter("id", administrativeAreal2DReference.Id);
            }

            httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            PostgreSQL.Classes.AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReferencePath>(json)?.FirstOrDefault();
            if (administrativeAreal2DReferencePath is null)
            {
                return NotFound();
            }

            administrativeAreal2DReferencePath.Remove(PostgreSQL.Enums.AdministrativeArealType.Subdivison);

            #endregion AdministrativeAreal2DReferencePath

            #region AdministrativeAreal2DReferences

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencesbycode");
            if (code is not null)
            {
                urlBuilder = urlBuilder.AddParameter("code", code);
            }

            PostgreSQL.Enums.AdministrativeArealType? administrativeArealType_Child = administrativeAreal2DReference.AdministrativeArealType.ChildAdministrativeArealType();
            if (administrativeArealType_Child is not null && administrativeArealType_Child.HasValue)
            {
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", administrativeArealType_Child.Value.ToString());
            }

            httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            List<PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReference>(json);
            if (administrativeAreal2DReferences is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2DReferences

            AdministrativeAreal2DView administrativeAreal2DView = new(administrativeAreal2DReference, administrativeAreal2D, administrativeAreal2DReferencePath, administrativeAreal2DReferences);

            return PartialView("_AdministrativeAreal2DView", administrativeAreal2DView);
        }

        [HttpGet("administrativeareal2Dreferencesbyid")]
        public async Task<IActionResult> GetAdministrativeAreal2DReferencesByIdAsync([FromQuery(Name = "id")] int id)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region AdministrativeAreal2DReferencePath

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencepathbyid");
            urlBuilder = urlBuilder.AddParameter("id", id);

            httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            PostgreSQL.Classes.AdministrativeAreal2DReferencePath? administrativeAreal2DReferencePath = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReferencePath>(json)?.FirstOrDefault();
            if (administrativeAreal2DReferencePath is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2DReferencePath

            #region AdministrativeAreal2DReference

            PostgreSQL.Classes.AdministrativeAreal2DReference? administrativeAreal2DReference = administrativeAreal2DReferencePath.AdministrativeAreal2DReferences?.Last();
            if (administrativeAreal2DReference is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2DReference

            #region AdministrativeAreal2D

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/itembyid");
            urlBuilder = urlBuilder.AddParameter("id", id);

            httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            AdministrativeAreal2D? administrativeAreal2D = Core.Convert.ToDiGi<AdministrativeAreal2D>(json)?.FirstOrDefault();
            if (administrativeAreal2D is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2D

            #region AdministrativeAreal2DReferences

            List<PostgreSQL.Classes.AdministrativeAreal2DReference>? administrativeAreal2DReferences = null;

            if(administrativeAreal2DReference.Code is string code && !string.IsNullOrWhiteSpace(code) && administrativeAreal2DReference?.AdministrativeArealType is PostgreSQL.Enums.AdministrativeArealType administrativeArealType && administrativeArealType.ChildAdministrativeArealType() is  PostgreSQL.Enums.AdministrativeArealType administrativeArealType_Child)
            {
                urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencesbycode");
                urlBuilder = urlBuilder.AddParameter("code", code);
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", administrativeArealType_Child.ToString());

                httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    return BadRequest();
                }

                json = await httpResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json))
                {
                    return NoContent();
                }

                administrativeAreal2DReferences = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReference>(json);
            }

            #endregion AdministrativeAreal2DReferences

            AdministrativeAreal2DView administrativeAreal2DView = new(administrativeAreal2DReference, administrativeAreal2D, administrativeAreal2DReferencePath, administrativeAreal2DReferences);

            return PartialView("_AdministrativeAreal2DView", administrativeAreal2DView);
        }
        
        [HttpGet("itembycode")]
        public async Task<IActionResult> GetItemByCodeAsync([FromQuery(Name = "code")] string code)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();
            string url = $"https://api.digiproject.uk/gis/administrativeareal2D/itembycode?code={code}";

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
            AdministrativeAreal2D? administrativeAreal2D = Core.Convert.ToDiGi<AdministrativeAreal2D>(json)?.FirstOrDefault();
            // We pass the object to a Partial View
            return PartialView("_AdministrativeAreal2DView", administrativeAreal2D);
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

        [HttpGet("pointsbyid")]
        public async Task<IActionResult> GetPointsByIdAsync([FromQuery(Name = "id")] int id, [FromQuery(Name = "reductionfactor")] double? reductionFactor = null, [FromQuery(Name = "mincount")] int? minCount = null)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            UrlBuilder urlBuilder;
            HttpResponseMessage httpResponseMessage;
            string json;

            #region AdministrativeAreal2DReference

            urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/administrativeareal2Dreferencebyid");
            urlBuilder = urlBuilder.AddParameter("id", id);

            httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            PostgreSQL.Classes.AdministrativeAreal2DReference? administrativeAreal2DReference = Core.Convert.ToDiGi<PostgreSQL.Classes.AdministrativeAreal2DReference>(json)?.FirstOrDefault();
            if (administrativeAreal2DReference is null)
            {
                return NotFound();
            }

            #endregion AdministrativeAreal2DReference

            List<AdministrativeAreal2D> administrativeAreal2Ds = [];

            if(administrativeAreal2DReference.AdministrativeArealType == PostgreSQL.Enums.AdministrativeArealType.Subdivison || administrativeAreal2DReference.AdministrativeArealType == PostgreSQL.Enums.AdministrativeArealType.Municipality)
            {
                #region AdministrativeAreal2D

                urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/itembyid");
                urlBuilder = urlBuilder.AddParameter("id", id);

                httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    return BadRequest();
                }

                json = await httpResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json))
                {
                    return NoContent();
                }

                AdministrativeAreal2D? administrativeAreal2D = Core.Convert.ToDiGi<AdministrativeAreal2D>(json)?.FirstOrDefault();
                if (administrativeAreal2D is null)
                {
                    return NotFound();
                }

                administrativeAreal2Ds.Add(administrativeAreal2D);

                #endregion AdministrativeAreal2D
            }
            else
            {
                #region AdministrativeAreal2Ds

                urlBuilder = new("https://api.digiproject.uk/gis/administrativeareal2D/itemsbycode");
                urlBuilder = urlBuilder.AddParameter("code", administrativeAreal2DReference.Code);
                urlBuilder = urlBuilder.AddParameter("administrativearealtype", administrativeAreal2DReference.AdministrativeArealType.ToString());

                httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString());
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    return BadRequest();
                }

                json = await httpResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json))
                {
                    return NoContent();
                }

                if(Core.Convert.ToDiGi<AdministrativeAreal2D>(json) is not List<AdministrativeAreal2D> administrativeAreal2Ds_Temp || administrativeAreal2Ds_Temp.Count == 0)
                {
                    return NotFound();
                }

                administrativeAreal2Ds = administrativeAreal2Ds_Temp;

                #endregion AdministrativeAreal2Ds
            }

            #region Point2Ds

            List<Point2D>? point2Ds = [];
            foreach (AdministrativeAreal2D? administrativeAreal2D in administrativeAreal2Ds)
            {
                List<Point2D>? point2Ds_Temp = administrativeAreal2D.PolygonalFace2D?.ExternalEdge?.GetPoints();
                Modify.Reduce(point2Ds_Temp, reductionFactor, minCount ?? 100);
                if (point2Ds_Temp is not null)
                {
                    point2Ds.AddRange(point2Ds_Temp);
                }
            }

            if(administrativeAreal2Ds.Count > 1)
            {
                point2Ds =  Geometry.Planar.Query.ConvexHull(point2Ds, false);
            }

            #endregion Point2Ds

            string result = point2Ds is null ? string.Empty : string.Join(" ", point2Ds.ConvertAll(p => $"{p.X} {p.Y}"));

            return Content(result, "text/plain");
        }

        // This action will trigger for: gis.digiproject.uk/administrativeareal2D
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }
    }
}