using DiGi.Analytical.Building.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.GLTF.Analytical;
using DiGi.GLTF.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides the 3D glTF viewer for GIS and analytical objects.
    /// <para>The viewer pages are lightweight shells: the geometry is streamed separately as a binary glTF (.glb) payload from the glb endpoints, whose scene extras are fully self-describing (batched geometry, object identity map, scene configuration).</para>
    /// </summary>
    [Route("[controller]")]
    public class GLTFController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLTFController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> used to create <see cref="HttpClient"/> instances.</param>
        public GLTFController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Renders the 3D viewer page for a single building. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint.
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="storeyHeight">The optional storey height in meters used for the extrusion.</param>
        /// <returns>An <see cref="IActionResult"/> rendering the glTF scene view.</returns>
        [HttpGet("building2dbyid")]
        public IActionResult GetBuilding2DById([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "storeyheight")] double? storeyHeight = null)
        {
            string gLBUrl = $"~/gltf/glb/building2dbyid?id={id.ToString(CultureInfo.InvariantCulture)}";
            if (countyId is not null && countyId.HasValue)
            {
                gLBUrl += $"&countyid={countyId.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            if (storeyHeight is not null && storeyHeight.HasValue)
            {
                gLBUrl += $"&storeyheight={storeyHeight.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            GLTFSceneViewModel gLTFSceneViewModel = new($"BuildingModel {id}", gLBUrl);

            return View("GLTFSceneView", gLTFSceneViewModel);
        }

        /// <summary>
        /// Asynchronously creates a <see cref="BuildingModel"/> for the building with the specified unique identifier (see <see cref="Create.BuildingModelAsync(HttpClient?, long, int?, double, double)"/>), converts each of its components (walls, floors and roofs) into a separate node of a batched <see cref="GLTFScene"/> (translated to a local origin) and streams it as a binary glTF (.glb) payload.
        /// <para>Each component carries its own identity in the scene object map, so the 3D viewer can hit-test and select individual components instead of the building as a whole.</para>
        /// </summary>
        /// <param name="id">The unique identifier of the building.</param>
        /// <param name="countyId">The optional unique identifier of the county associated with the building.</param>
        /// <param name="storeyHeight">The optional storey height in meters used for the extrusion.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the .glb file.</returns>
        [HttpGet("glb/building2dbyid")]
        public async Task<IActionResult> GetBuilding2DGLBByIdAsync([FromQuery(Name = "id")] long id, [FromQuery(Name = "countyid")] int? countyId, [FromQuery(Name = "storeyheight")] double? storeyHeight = null)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            BuildingModel? buildingModel = await httpClient.BuildingModelAsync(id, countyId, storeyHeight ?? Constants.Default.StoreyHeight);
            if (buildingModel is null)
            {
                return NoContent();
            }

            List<GLTFNode>? gLTFNodes = buildingModel.ToGLTF_GLTFNodes();
            if (gLTFNodes is null || gLTFNodes.Count == 0)
            {
                return NoContent();
            }

            string name = $"BuildingModel {id.ToString(CultureInfo.InvariantCulture)}";

            GLTFScene? gLTFScene = GLTF.Create.GLTFScene(gLTFNodes, name);
            if (gLTFScene is null)
            {
                return NoContent();
            }

            byte[]? bytes = GLTF.Convert.ToSystem_Bytes(gLTFScene, true);
            if (bytes is null || bytes.Length == 0)
            {
                return NoContent();
            }

            return File(bytes, "model/gltf-binary", $"{name}.glb");
        }

        // This action will trigger for: gis.digiproject.uk/gltf
        /// <summary>
        /// Initializes and returns the start view for the 3D glTF viewer.
        /// </summary>
        /// <returns>An <see cref="Microsoft.AspNetCore.Mvc.IActionResult"/> result that renders the starting view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }
    }
}
