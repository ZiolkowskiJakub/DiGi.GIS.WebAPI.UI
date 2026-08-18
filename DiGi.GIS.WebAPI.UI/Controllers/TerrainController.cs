using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.GLTF.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides the terrain feature: the ground surface of an area as a <see cref="Mesh3D"/>, as a binary glTF payload and as a 3D viewer page.
    /// <para>The surface itself is reconstructed by the GIS Web API terrain endpoints (gis/terrain) from the elevation points stored per county; this controller only relays them, so the query limits (maximum radius, mesh edge length, tolerance defaults) stay owned by that service and cannot drift here.</para>
    /// <para>Every surface returned here is a two-and-a-half dimensional height field: exactly one elevation per plan position. It models ground, and cannot express a vertical face, an overhang or a canopy.</para>
    /// </summary>
    [Route("[controller]")]
    public class TerrainController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TerrainController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> used to create <see cref="HttpClient"/> instances.</param>
        public TerrainController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Asynchronously retrieves the terrain surface inside a circle centred on the given plan coordinate.
        /// <para>Either <paramref name="radius"/> or <paramref name="diameter"/> must be supplied; <paramref name="radius"/> wins when both are.</para>
        /// <para>A terrain payload is optional by contract: when the area holds no stored elevation points, or the terrain service cannot answer, the response is 204 rather than an error - see <see cref="Query.TerrainJsonAsync(HttpClient, string, CancellationToken)"/>.</para>
        /// </summary>
        /// <param name="x">The X coordinate of the centre, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y">The Y coordinate of the centre, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="radius">The search radius in metres. Optional when <paramref name="diameter"/> is supplied.</param>
        /// <param name="diameter">The search diameter in metres, used only when <paramref name="radius"/> is absent.</param>
        /// <param name="tolerance">An optional tolerance for the spatial query, in metres. When omitted the terrain service applies its own default.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> carrying the <see cref="Mesh3D"/> as JSON.</returns>
        [HttpGet("mesh3dbycircle")]
        public async Task<IActionResult> GetMesh3DByCircleAsync([FromQuery(Name = "x")] double x, [FromQuery(Name = "y")] double y, [FromQuery(Name = "radius")] double? radius, [FromQuery(Name = "diameter")] double? diameter, [FromQuery(Name = "tolerance")] double? tolerance, CancellationToken cancellationToken = default)
        {
            string? requestUri = Circle2D(x, y, radius, diameter).TerrainRequestUri(tolerance);
            if (requestUri is null)
            {
                return BadRequest();
            }

            // Relayed verbatim rather than round tripped through Mesh3D: the deserialization and the
            // reserialization would reallocate a payload of up to some 160 000 points to produce the
            // very same bytes.
            string? json = await httpClientFactory.CreateClient().TerrainJsonAsync(requestUri, cancellationToken);
            if (json is null)
            {
                return NoContent();
            }

            return Content(json, "application/json");
        }

        /// <summary>
        /// Asynchronously retrieves the terrain surface inside an axis aligned bounding box given by two opposite corners.
        /// <para>Corner order does not matter.</para>
        /// <para>A terrain payload is optional by contract: when the area holds no stored elevation points, or the terrain service cannot answer, the response is 204 rather than an error - see <see cref="Query.TerrainJsonAsync(HttpClient, string, CancellationToken)"/>.</para>
        /// </summary>
        /// <param name="x_1">The X coordinate of the first corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y_1">The Y coordinate of the first corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="x_2">The X coordinate of the second corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y_2">The Y coordinate of the second corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="tolerance">An optional tolerance for the spatial query, in metres. When omitted the terrain service applies its own default.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> carrying the <see cref="Mesh3D"/> as JSON.</returns>
        [HttpGet("mesh3dbyboundingbox")]
        public async Task<IActionResult> GetMesh3DByBoundingBoxAsync([FromQuery(Name = "x_1")] double x_1, [FromQuery(Name = "y_1")] double y_1, [FromQuery(Name = "x_2")] double x_2, [FromQuery(Name = "y_2")] double y_2, [FromQuery(Name = "tolerance")] double? tolerance, CancellationToken cancellationToken = default)
        {
            string? requestUri = new BoundingBox2D(new Point2D(x_1, y_1), new Point2D(x_2, y_2)).TerrainRequestUri(tolerance);
            if (requestUri is null)
            {
                return BadRequest();
            }

            string? json = await httpClientFactory.CreateClient().TerrainJsonAsync(requestUri, cancellationToken);
            if (json is null)
            {
                return NoContent();
            }

            return Content(json, "application/json");
        }

        /// <summary>
        /// Asynchronously retrieves the terrain surface inside a circle and streams it as a binary glTF (.glb) payload for the 3D viewer.
        /// </summary>
        /// <param name="x">The X coordinate of the centre, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y">The Y coordinate of the centre, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="radius">The search radius in metres. Optional when <paramref name="diameter"/> is supplied.</param>
        /// <param name="diameter">The search diameter in metres, used only when <paramref name="radius"/> is absent.</param>
        /// <param name="tolerance">An optional tolerance for the spatial query, in metres. When omitted the terrain service applies its own default.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the .glb file.</returns>
        [HttpGet("glb/mesh3dbycircle")]
        public async Task<IActionResult> GetGLBMesh3DByCircleAsync([FromQuery(Name = "x")] double x, [FromQuery(Name = "y")] double y, [FromQuery(Name = "radius")] double? radius, [FromQuery(Name = "diameter")] double? diameter, [FromQuery(Name = "tolerance")] double? tolerance, CancellationToken cancellationToken = default)
        {
            string? requestUri = Circle2D(x, y, radius, diameter).TerrainRequestUri(tolerance);
            if (requestUri is null)
            {
                return BadRequest();
            }

            return await GLBResultAsync(requestUri, CircleName(x, y, radius, diameter), new Point3D(x, y, 0), cancellationToken);
        }

        /// <summary>
        /// Asynchronously retrieves the terrain surface inside an axis aligned bounding box and streams it as a binary glTF (.glb) payload for the 3D viewer.
        /// </summary>
        /// <param name="x_1">The X coordinate of the first corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y_1">The Y coordinate of the first corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="x_2">The X coordinate of the second corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y_2">The Y coordinate of the second corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="tolerance">An optional tolerance for the spatial query, in metres. When omitted the terrain service applies its own default.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the .glb file.</returns>
        [HttpGet("glb/mesh3dbyboundingbox")]
        public async Task<IActionResult> GetGLBMesh3DByBoundingBoxAsync([FromQuery(Name = "x_1")] double x_1, [FromQuery(Name = "y_1")] double y_1, [FromQuery(Name = "x_2")] double x_2, [FromQuery(Name = "y_2")] double y_2, [FromQuery(Name = "tolerance")] double? tolerance, CancellationToken cancellationToken = default)
        {
            string? requestUri = new BoundingBox2D(new Point2D(x_1, y_1), new Point2D(x_2, y_2)).TerrainRequestUri(tolerance);
            if (requestUri is null)
            {
                return BadRequest();
            }

            return await GLBResultAsync(requestUri, BoundingBoxName(x_1, y_1, x_2, y_2), new Point3D((x_1 + x_2) / 2, (y_1 + y_2) / 2, 0), cancellationToken);
        }

        /// <summary>
        /// Renders the 3D viewer page for the terrain inside a circle. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint.
        /// </summary>
        /// <param name="centerX">The X coordinate of the centre of the requested area.</param>
        /// <param name="centerY">The Y coordinate of the centre of the requested area.</param>
        /// <param name="radius">The radius of the requested area in metres.</param>
        /// <returns>An <see cref="IActionResult"/> rendering the glTF scene view.</returns>
        [HttpGet("terrainbycircle")]
        public IActionResult GetTerrainByCircle([FromQuery(Name = "centerX")] double centerX, [FromQuery(Name = "centerY")] double centerY, [FromQuery(Name = "radius")] double radius)
        {
            if (!double.IsFinite(centerX) || !double.IsFinite(centerY) || !double.IsFinite(radius) || radius <= 0)
            {
                return BadRequest();
            }

            string gLBUrl = $"~/terrain/glb/mesh3dbycircle?x={centerX.ToString(CultureInfo.InvariantCulture)}&y={centerY.ToString(CultureInfo.InvariantCulture)}&radius={radius.ToString(CultureInfo.InvariantCulture)}";

            // No scope box preset: terrain is a single flat sheet, so the viewer's bounds fit is the
            // right default (the multi building scenes preset one because their extent is unbounded).
            GLTFSceneViewModel gLTFSceneViewModel = new(CircleName(centerX, centerY, radius, null), gLBUrl);

            return View("~/Views/GLTF/GLTFSceneView.cshtml", gLTFSceneViewModel);
        }

        /// <summary>
        /// Renders the 3D viewer page for the terrain inside an axis aligned bounding box. The page itself carries no geometry; the viewer streams the binary glTF payload from the glb endpoint.
        /// </summary>
        /// <param name="x_1">The X coordinate of the first corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y_1">The Y coordinate of the first corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="x_2">The X coordinate of the second corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y_2">The Y coordinate of the second corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <returns>An <see cref="IActionResult"/> rendering the glTF scene view.</returns>
        [HttpGet("terrainbyboundingbox")]
        public IActionResult GetTerrainByBoundingBox([FromQuery(Name = "x_1")] double x_1, [FromQuery(Name = "y_1")] double y_1, [FromQuery(Name = "x_2")] double x_2, [FromQuery(Name = "y_2")] double y_2)
        {
            if (!double.IsFinite(x_1) || !double.IsFinite(y_1) || !double.IsFinite(x_2) || !double.IsFinite(y_2))
            {
                return BadRequest();
            }

            string gLBUrl = $"~/terrain/glb/mesh3dbyboundingbox?x_1={x_1.ToString(CultureInfo.InvariantCulture)}&y_1={y_1.ToString(CultureInfo.InvariantCulture)}&x_2={x_2.ToString(CultureInfo.InvariantCulture)}&y_2={y_2.ToString(CultureInfo.InvariantCulture)}";

            GLTFSceneViewModel gLTFSceneViewModel = new(BoundingBoxName(x_1, y_1, x_2, y_2), gLBUrl);

            return View("~/Views/GLTF/GLTFSceneView.cshtml", gLTFSceneViewModel);
        }

        // This action will trigger for: gis.digiproject.uk/terrain
        /// <summary>
        /// Initializes and returns the start view of the terrain feature.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> result that renders the starting view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }

        /// <summary>
        /// Converts the terrain surface behind the given request into a batched <see cref="GLTFScene"/> and renders it as a binary glTF (.glb) response body.
        /// <para>Missing terrain is answered with 204 at every step. The viewer treats an empty payload as "nothing to draw" and keeps the rest of the page working, so a request for an area the terrain store does not cover yet degrades instead of failing.</para>
        /// </summary>
        /// <param name="requestUri">The terrain service URL to read the surface from.</param>
        /// <param name="name">The name given to the scene and to its single node.</param>
        /// <param name="referencePoint">The world point the scene is translated to a local origin around. The centre of the requested area, matching what the building scenes use, so a terrain scene and a building scene of the same area share one local origin.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the .glb file, or a no content status.</returns>
        private async Task<IActionResult> GLBResultAsync(string requestUri, string name, Point3D referencePoint, CancellationToken cancellationToken)
        {
            GLTFNode? gLTFNode = await httpClientFactory.CreateClient().TerrainGLTFNodeAsync(requestUri, name, cancellationToken);
            if (gLTFNode is null)
            {
                return NoContent();
            }

            GLTFScene? gLTFScene = GLTF.Create.GLTFScene([gLTFNode], name, referencePointOverride: referencePoint);
            if (gLTFScene is null)
            {
                return NoContent();
            }

            byte[]? bytes = GLTF.Convert.ToSystem_Bytes(gLTFScene, true);
            if (bytes is null || bytes.Length == 0)
            {
                return NoContent();
            }

            return File(bytes, "model/gltf-binary", "terrain.glb");
        }

        /// <summary>
        /// Builds the circular area a request asked for, resolving the radius from either the radius or the diameter.
        /// <para>An area that cannot be served (a coordinate or a radius that is not a usable number) is built all the same and rejected afterwards by <see cref="Query.TerrainRequestUri(Circle2D, double?)"/>, so what a terrain request may ask for is decided in one place.</para>
        /// </summary>
        /// <param name="x">The X coordinate of the centre, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y">The Y coordinate of the centre, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="radius">The search radius in metres. Optional when <paramref name="diameter"/> is supplied.</param>
        /// <param name="diameter">The search diameter in metres, used only when <paramref name="radius"/> is absent.</param>
        /// <returns>The requested area.</returns>
        private static Circle2D Circle2D(double x, double y, double? radius, double? diameter)
        {
            double radius_Temp = double.NaN;
            if (radius.HasValue && !double.IsNaN(radius.Value))
            {
                radius_Temp = radius.Value;
            }
            else if (diameter.HasValue && !double.IsNaN(diameter.Value))
            {
                radius_Temp = diameter.Value / 2;
            }

            return new Circle2D(new Point2D(x, y), radius_Temp);
        }

        /// <summary>
        /// Builds the name of a circular terrain area, used as the scene name and as the viewer page title.
        /// </summary>
        /// <param name="x">The X coordinate of the centre, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y">The Y coordinate of the centre, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="radius">The search radius in metres. Optional when <paramref name="diameter"/> is supplied.</param>
        /// <param name="diameter">The search diameter in metres, used only when <paramref name="radius"/> is absent.</param>
        /// <returns>The name of the area.</returns>
        private static string CircleName(double x, double y, double? radius, double? diameter)
        {
            return $"{Constants.Default.TerrainName} ({x}, {y}) r = {Circle2D(x, y, radius, diameter).Radius} m";
        }

        /// <summary>
        /// Builds the name of a rectangular terrain area, used as the scene name and as the viewer page title.
        /// </summary>
        /// <param name="x_1">The X coordinate of the first corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y_1">The Y coordinate of the first corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="x_2">The X coordinate of the second corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <param name="y_2">The Y coordinate of the second corner, in PL-1992 (EPSG:2180) metres.</param>
        /// <returns>The name of the area.</returns>
        private static string BoundingBoxName(double x_1, double y_1, double x_2, double y_2)
        {
            return $"{Constants.Default.TerrainName} ({x_1}, {y_1}) - ({x_2}, {y_2})";
        }
    }
}
