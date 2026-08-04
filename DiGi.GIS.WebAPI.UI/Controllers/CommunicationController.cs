using DiGi.Analytical.Building.Classes;
using DiGi.Communication.Classes;
using DiGi.Communication.Enums;
using DiGi.Communication.WebAPI;
using DiGi.Communication.WebAPI.Classes;
using DiGi.Core.Constants;
using DiGi.Geometry.Spatial.Classes;
using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// Provides the communication analysis feature: an input page for the analyzed circular area, the 3D scene view with the antenna toolbar and the calculation endpoint that fetches the analyzed area buildings and solves the radio propagation between the placed antennas in process.
    /// </summary>
    [Route("[controller]")]
    public class CommunicationController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> used to create <see cref="HttpClient"/> instances.</param>
        public CommunicationController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Renders the communication 3D scene view for all buildings within the specified circular area (streamed as a binary glTF payload from the existing glb endpoint) together with the antenna toolbar.
        /// </summary>
        /// <param name="centerX">The X coordinate of the center of the analyzed circular area.</param>
        /// <param name="centerY">The Y coordinate of the center of the analyzed circular area.</param>
        /// <param name="radius">The radius of the analyzed circular area in meters.</param>
        /// <returns>An <see cref="IActionResult"/> rendering the communication scene view.</returns>
        [HttpGet("buildingsbyradius")]
        public IActionResult GetBuildingsByRadius([FromQuery(Name = "centerX")] double centerX, [FromQuery(Name = "centerY")] double centerY, [FromQuery(Name = "radius")] double radius)
        {
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || double.IsNaN(radius) || radius <= 0)
            {
                return BadRequest("The analyzed area center coordinates and radius must be valid positive numbers.");
            }

            string gLBUrl = $"~/buildingmodel/glb/buildingsbyradius?centerX={centerX.ToString(CultureInfo.InvariantCulture)}&centerY={centerY.ToString(CultureInfo.InvariantCulture)}&radius={radius.ToString(CultureInfo.InvariantCulture)}";

            string title = $"Buildings ({centerX}, {centerY}) r = {radius} m";

            CommunicationSceneViewModel communicationSceneViewModel = new(title, gLBUrl, centerX, centerY, radius);

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

        /// <summary>
        /// Executes the communication calculation for the antennas placed in the 3D view.
        /// <para>The buildings of the analyzed area are fetched as <see cref="BuildingModel"/> instances and converted to <see cref="ScatteringObject"/> instances, packaged together with the antennas into a <see cref="GeometricalPropagationModel"/> and solved in process (<see cref="ScatteringSolver"/> + <see cref="AngularPowerDistributionSolver"/>); nothing is persisted.</para>
        /// </summary>
        /// <param name="communicationCalculationParameter">The analyzed circular area and the antennas placed by the user.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the <see cref="GeometricalPropagationResult"/> JSON grouped by delay (ascending): the propagation ellipsoids, the scattering polylines (one per <see cref="ScatteringPointGroup"/>) and the angular power distribution vectors, all in world coordinates.</returns>
        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateAsync([FromBody] CommunicationCalculationParameter? communicationCalculationParameter, CancellationToken cancellationToken = default)
        {
            if (communicationCalculationParameter is null || communicationCalculationParameter.Antennas is null || communicationCalculationParameter.Antennas.Count != 2)
            {
                return BadRequest("Two antennas (transmitter and receiver) are required.");
            }

            if (double.IsNaN(communicationCalculationParameter.CenterX) || double.IsNaN(communicationCalculationParameter.CenterY) || double.IsNaN(communicationCalculationParameter.Radius) || communicationCalculationParameter.Radius <= 0)
            {
                return BadRequest("The analyzed area center coordinates and radius must be valid positive numbers.");
            }

            try
            {
                HttpClient httpClient = httpClientFactory.CreateClient();

                #region BuildingModels

                // The buildings are fetched on the fly for the analyzed area (no database storage on the
                // communication side) and reduced to plain triangulated geometry so no GIS type ever
                // crosses the DiGi.Communication.WebAPI boundary.
                UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/buildingmodel/itemsbycircle");
                urlBuilder = urlBuilder.AddParameter("x", communicationCalculationParameter.CenterX);
                urlBuilder = urlBuilder.AddParameter("y", communicationCalculationParameter.CenterY);
                urlBuilder = urlBuilder.AddParameter("radius", communicationCalculationParameter.Radius);

                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(urlBuilder.ToString(), cancellationToken);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    return BadRequest($"Failed to fetch buildings from the GIS service (HTTP {(int)httpResponseMessage.StatusCode}).");
                }

                string json = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return NoContent();
                }

                List<BuildingModel>? buildingModels = Core.Convert.ToDiGi<BuildingModel>(json);

                if (buildingModels is null || buildingModels.Count == 0)
                {
                    return NoContent();
                }

                #endregion BuildingModels

                GeometricalPropagationModel geometricalPropagationModel = new();

                double minElevation = double.MaxValue;

                #region ScatteringObjects

                foreach (BuildingModel buildingModel in buildingModels)
                {
                    List<ScatteringObject>? scatteringObjects = buildingModel?.ToCommunication();
                    if (scatteringObjects is null || scatteringObjects.Count == 0)
                    {
                        continue;
                    }

                    ScatteringGroup? scatteringGroup = geometricalPropagationModel.Group(buildingModel!.Guid.ToString(), scatteringObjects);
                    if (scatteringGroup is not null && scatteringGroup?.BoundingBox3D?.Min.Z is double elevation && minElevation > elevation)
                    {
                        minElevation = elevation;
                    }
                }

                #endregion ScatteringObjects

                if (minElevation == double.MaxValue)
                {
                    minElevation = 0;
                }

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

                #region SimpleMultipathPowerDelayProfile

                DefaultSimpleMultipathPowerDelayProfile profile = DefaultSimpleMultipathPowerDelayProfile.TypicalUrban;
                if (!string.IsNullOrWhiteSpace(communicationCalculationParameter.DefaultSimpleMultipathPowerDelayProfile))
                {
                    _ = Enum.TryParse(communicationCalculationParameter.DefaultSimpleMultipathPowerDelayProfile, true, out profile);
                }

                SimpleMultipathPowerDelayProfile? simpleMultipathPowerDelayProfile = Communication.Create.SimpleMultipathPowerDelayProfile(profile);

                geometricalPropagationModel.Assign(simpleMultipathPowerDelayProfile, antennas[0], antennas[1]);

                #endregion SimpleMultipathPowerDelayProfile

                #region Transmitter/receiver selection

                // The selection below mirrors DiGi.Communication
                // Convert.ToPropagation_PropagationModel (first antenna with the Transmitter function,
                // first other antenna with the Receiver function); validated before the solvers run so
                // an invalid antenna setup fails fast.
                Antenna? antenna_Transmitter = antennas.Find(x => x.Location is not null && x.Functions?.Contains(Function.Transmitter) == true);
                Antenna? antenna_Receiver = antennas.Find(x => x.Guid != antenna_Transmitter?.Guid && x.Location is not null && x.Functions?.Contains(Function.Receiver) == true);

                Point3D? location_Transmitter = antenna_Transmitter?.Location;
                Point3D? location_Receiver = antenna_Receiver?.Location;
                if (location_Transmitter is null || location_Receiver is null)
                {
                    return BadRequest("Transmitter and receiver antennas with valid locations are required.");
                }

                double distance = location_Transmitter.Distance(location_Receiver);
                if (distance <= 0)
                {
                    return BadRequest("Transmitter and receiver antennas must be at different locations.");
                }

                #endregion Transmitter/receiver selection

                #region Solvers

                ScatteringSolver scatteringSolver = new()
                {
                    GeometricalPropagationModel = geometricalPropagationModel,
                    ScatteringSolverOptions = new ScatteringSolverOptions(Communication.Constants.Factor.Angle, 0.1, Tolerance.Distance)
                };

                scatteringSolver.Solve();

                // The frequency [Hz] is collected by the calculation modal (in MHz, converted before it is
                // sent); an omitted or invalid value falls back to the AngularPowerDistributionSolverOptions
                // default rather than failing the request.
                AngularPowerDistributionSolverOptions angularPowerDistributionSolverOptions = new();
                if (communicationCalculationParameter.Frequency is double frequency && !double.IsNaN(frequency) && frequency > 0)
                {
                    angularPowerDistributionSolverOptions.Frequency = frequency;
                }

                AngularPowerDistributionSolver angularPowerDistributionSolver = new()
                {
                    GeometricalPropagationModel = geometricalPropagationModel,
                    AngularPowerDistributionSolverOptions = angularPowerDistributionSolverOptions
                };

                angularPowerDistributionSolver.Solve();

                #endregion Solvers

                #region GeometricalPropagationResult creation

                // The payload is projected by DiGi.Communication.WebAPI: it is GIS agnostic and it is
                // where the propagation calculation itself is heading, so the result contract lives
                // next to the calculation rather than in this application. Everything it holds is
                // expressed in world coordinates and grouped by delay; see renderDelayResults in
                // communication-tools.js for how the 3D view consumes it.
                GeometricalPropagationResult? geometricalPropagationResult = geometricalPropagationModel.GeometricalPropagationResult(location_Transmitter, location_Receiver, minElevation);
                if (geometricalPropagationResult is null)
                {
                    return NoContent();
                }

                #endregion GeometricalPropagationResult creation

                return Json(geometricalPropagationResult);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return StatusCode(499, "Calculation cancelled by the client.");
            }
            catch (TaskCanceledException exception)
            {
                return StatusCode(504, $"Building data request timed out: {exception.Message}");
            }
            catch (Exception exception)
            {
                return StatusCode(500, $"Internal server error: {exception.GetType().Name}: {exception.Message}");
            }
        }
    }
}
