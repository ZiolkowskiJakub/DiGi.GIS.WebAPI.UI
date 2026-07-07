using DiGi.Communication.Classes;
using DiGi.Communication.Enums;
using DiGi.Geometry.Spatial.Classes;
using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides the communication analysis feature: an input page for the analyzed circular area, the 3D scene view with the antenna toolbar and the calculation endpoint bridging the 3D view with the GIS agnostic DiGi.Communication.WebAPI service.
    /// </summary>
    [Route("[controller]")]
    public class CommunicationController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IWebHostEnvironment webHostEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> used to create <see cref="HttpClient"/> instances.</param>
        /// <param name="webHostEnvironment">The <see cref="IWebHostEnvironment"/> used to select the DiGi.Communication.WebAPI base URI.</param>
        public CommunicationController(IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            this.httpClientFactory = httpClientFactory;
            this.webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Executes the communication calculation for the antennas placed in the 3D view.
        /// <para>The buildings of the analyzed area are fetched and converted on the fly (Building -> Mesh3D -> ScatteringObject), packaged together with the antennas into a <see cref="GeometricalPropagationModel"/> and sent to the GIS agnostic DiGi.Communication.WebAPI service; nothing is persisted.</para>
        /// </summary>
        /// <param name="communicationCalculationParameter">The analyzed circular area and the antennas placed by the user.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the calculation result JSON (currently the connecting <see cref="Segment3D"/> and its length).</returns>
        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateAsync([FromBody] CommunicationCalculationParameter? communicationCalculationParameter, CancellationToken cancellationToken = default)
        {
            if (communicationCalculationParameter is null || communicationCalculationParameter.Antennas is null || communicationCalculationParameter.Antennas.Count != 2)
            {
                return BadRequest();
            }

            if (double.IsNaN(communicationCalculationParameter.CenterX) || double.IsNaN(communicationCalculationParameter.CenterY) || double.IsNaN(communicationCalculationParameter.Radius) || communicationCalculationParameter.Radius <= 0)
            {
                return BadRequest();
            }

            HttpClient httpClient = httpClientFactory.CreateClient();

            #region Building2Ds -> ScatteringObjects

            // The buildings are fetched on the fly for the analyzed area (no database storage on the
            // communication side) and reduced to plain triangulated geometry so no GIS type ever
            // crosses the DiGi.Communication.WebAPI boundary.
            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/building2D/itemsbycircle");
            urlBuilder = urlBuilder.AddParameter("x", communicationCalculationParameter.CenterX);
            urlBuilder = urlBuilder.AddParameter("y", communicationCalculationParameter.CenterY);
            urlBuilder = urlBuilder.AddParameter("radius", communicationCalculationParameter.Radius);

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString(), cancellationToken);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            string json = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            List<GIS.Classes.Building2D>? building2Ds = Core.Convert.ToDiGi<GIS.Classes.Building2D>(json);

            List<ScatteringObject>? scatteringObjects = building2Ds.ToCommunication_ScatteringObjects(communicationCalculationParameter.StoreyHeight ?? Constants.Default.StoreyHeight);

            #endregion Building2Ds -> ScatteringObjects

            #region Antennas

            List<Antenna> antennas = [];
            foreach (AntennaParameter antennaParameter in communicationCalculationParameter.Antennas)
            {
                List<Function> functions = [];
                if (antennaParameter.Functions is not null)
                {
                    foreach (string functionName in antennaParameter.Functions)
                    {
                        if (Enum.TryParse(functionName, true, out Function function) && !functions.Contains(function))
                        {
                            functions.Add(function);
                        }
                    }
                }

                antennas.Add(new Antenna(new Point3D(antennaParameter.X, antennaParameter.Y, antennaParameter.Z), [.. functions]));
            }

            #endregion Antennas

            #region GeometricalPropagationModel

            GeometricalPropagationModel geometricalPropagationModel = new();

            // AI-NOTE (placeholder profile): the final implementation will let the user pick or
            // configure the multipath power delay profile; until then the TypicalUrban preset keeps
            // the payload identical in shape to the one produced by the Rhino/Grasshopper component
            // (DiGi.Communication.Rhino GeometricalPropagationModel).
            SimpleMultipathPowerDelayProfile? simpleMultipathPowerDelayProfile = Communication.Create.SimpleMultipathPowerDelayProfile(DefaultSimpleMultipathPowerDelayProfile.TypicalUrban);

            geometricalPropagationModel.Assign(simpleMultipathPowerDelayProfile, antennas[0], antennas[1]);

            if (scatteringObjects is not null)
            {
                foreach (ScatteringObject scatteringObject in scatteringObjects)
                {
                    geometricalPropagationModel.Update(scatteringObject);
                }
            }

            #endregion GeometricalPropagationModel

            #region DiGi.Communication.WebAPI call

            // AI-NOTE (final endpoint call): this is where the final propagation calculation will be
            // requested. The GeometricalPropagationModel (buildings as ScatteringObject instances +
            // antennas) is sent to DiGi.Communication.WebAPI, the server calculates on the fly and
            // returns calculation objects (scattering profiles, rays, power delay profiles) which the
            // 3D view will render. Until then the temporary segment3d endpoint returns a Segment3D
            // connecting the two antenna tops.
            string communicationWebAPIUri = webHostEnvironment.IsDevelopment() ? Constants.Default.CommunicationWebAPIUri_Development : Constants.Default.CommunicationWebAPIUri;

            string? json_GeometricalPropagationModel = Core.Convert.ToSystem_String(geometricalPropagationModel);
            if (string.IsNullOrWhiteSpace(json_GeometricalPropagationModel))
            {
                return NoContent();
            }

            using StringContent stringContent = new(json_GeometricalPropagationModel, Encoding.UTF8, "application/json");

            httpResponseMessage = await httpClient.PostAsync($"{communicationWebAPIUri}/communication/geometricalpropagationmodel/segment3d", stringContent, cancellationToken);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            Segment3D? segment3D = Core.Convert.ToDiGi<Segment3D>(json)?.FirstOrDefault();

            Point3D? point3D_Start = segment3D?.Start;
            Point3D? point3D_End = segment3D?.End;
            if (segment3D is null || point3D_Start is null || point3D_End is null)
            {
                return NoContent();
            }

            #endregion DiGi.Communication.WebAPI call

            // Temporary result payload: the connecting segment and its length rendered by the 3D
            // view. It will be replaced by the calculation objects of the full propagation result.
            return Json(new
            {
                distance = segment3D.Length,
                start = new { x = point3D_Start.X, y = point3D_Start.Y, z = point3D_Start.Z },
                end = new { x = point3D_End.X, y = point3D_End.Y, z = point3D_End.Z }
            });
        }

        /// <summary>
        /// Renders the communication 3D scene view for all buildings within the specified circular area (streamed as a binary glTF payload from the existing glb endpoint) together with the antenna toolbar.
        /// </summary>
        /// <param name="centerX">The X coordinate of the center of the analyzed circular area.</param>
        /// <param name="centerY">The Y coordinate of the center of the analyzed circular area.</param>
        /// <param name="radius">The radius of the analyzed circular area in meters.</param>
        /// <param name="storeyHeight">The optional storey height in meters used for the building extrusions.</param>
        /// <returns>An <see cref="IActionResult"/> rendering the communication scene view.</returns>
        [HttpGet("buildingsbyradius")]
        public IActionResult GetBuildingsByRadius(
            [FromQuery(Name = "centerX")] double centerX,
            [FromQuery(Name = "centerY")] double centerY,
            [FromQuery(Name = "radius")] double radius,
            [FromQuery(Name = "storeyheight")] double? storeyHeight = null)
        {
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || double.IsNaN(radius) || radius <= 0)
            {
                return BadRequest();
            }

            string gLBUrl = $"~/gltf/glb/buildingsbyradius?centerX={centerX.ToString(CultureInfo.InvariantCulture)}&centerY={centerY.ToString(CultureInfo.InvariantCulture)}&radius={radius.ToString(CultureInfo.InvariantCulture)}";
            if (storeyHeight is not null && storeyHeight.HasValue)
            {
                gLBUrl += $"&storeyheight={storeyHeight.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            string title = $"Buildings ({centerX}, {centerY}) r = {radius} m";

            CommunicationSceneViewModel communicationSceneViewModel = new(title, gLBUrl, centerX, centerY, radius, storeyHeight ?? Constants.Default.StoreyHeight);

            return View("CommunicationSceneView", communicationSceneViewModel);
        }

        // This action will trigger for: gis.digiproject.uk/communication
        /// <summary>
        /// Initializes and returns the start view of the communication analysis feature.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> result that renders the starting view.</returns>
        [HttpGet("")]
        public IActionResult Start()
        {
            return View();
        }
    }
}
