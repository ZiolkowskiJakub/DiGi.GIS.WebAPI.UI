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

            #region Transmitter/receiver selection and model coordinate system

            // The selection below mirrors DiGi.Communication.Propagation
            // Convert.ToPropagation_PropagationModel (first antenna with the Transmitter function,
            // first other antenna with the Receiver function), so the world geometry rendered by the
            // 3D view matches the model coordinate system used by the calculation.
            Antenna? antenna_Transmitter = antennas.Find(x => x.Location is not null && x.Functions?.Contains(Function.Transmitter) == true);
            Antenna? antenna_Receiver = antennas.Find(x => x.Guid != antenna_Transmitter?.Guid && x.Location is not null && x.Functions?.Contains(Function.Receiver) == true);

            Point3D? location_Transmitter = antenna_Transmitter?.Location;
            Point3D? location_Receiver = antenna_Receiver?.Location;
            if (location_Transmitter is null || location_Receiver is null)
            {
                return BadRequest();
            }

            double distance = location_Transmitter.Distance(location_Receiver);
            if (distance <= 0)
            {
                return BadRequest();
            }

            // Orthonormal basis of the model coordinate system in world coordinates (must stay in
            // sync with Convert.ToPropagation_PropagationModel): the transmitter at the origin, the
            // OX axis towards the receiver and the OZ axis as close to the world vertical as possible.
            Vector3D? vector3D_AxisX = (location_Receiver - location_Transmitter)?.Unit;
            if (vector3D_AxisX is null)
            {
                return BadRequest();
            }

            Vector3D vector3D_Up = new(0, 0, 1);

            Vector3D? vector3D_AxisZ = vector3D_Up - (vector3D_AxisX * vector3D_Up.DotProduct(vector3D_AxisX));
            if (vector3D_AxisZ is null || vector3D_AxisZ.Length == 0)
            {
                return BadRequest();
            }

            vector3D_AxisZ = vector3D_AxisZ.Unit;

            Vector3D? vector3D_AxisY = vector3D_AxisZ?.CrossProduct(vector3D_AxisX);
            if (vector3D_AxisZ is null || vector3D_AxisY is null)
            {
                return BadRequest();
            }

            #endregion Transmitter/receiver selection and model coordinate system

            #region Calculation parameters

            // AI-NOTE (placeholder defaults): the fallback values below mirror the reference xUnit
            // fact (DiGi.Communication.Propagation.xUnit Facts.ToPropagation_PropagationModel_TypicalUrban):
            // 900 MHz, vertical polarization and the 15 / 0.005 material. They apply only when the
            // 3D view modal does not provide the values; replace them with user/project settings
            // once available.
            List<double> frequencies = communicationCalculationParameter.Frequencies?.FindAll(x => !double.IsNaN(x) && x > 0) ?? [];
            if (frequencies.Count == 0)
            {
                frequencies.Add(900);
            }

            string polarization = string.IsNullOrWhiteSpace(communicationCalculationParameter.Polarization) ? "Vertical" : communicationCalculationParameter.Polarization;
            double relativePermittivity = communicationCalculationParameter.RelativePermittivity ?? 15;
            double conductivity = communicationCalculationParameter.Conductivity ?? 0.005;

            #endregion Calculation parameters

            #region DiGi.Communication.WebAPI call

            // The GeometricalPropagationModel (buildings as ScatteringObject instances + antennas) is
            // sent to the GIS agnostic DiGi.Communication.WebAPI propagationresults endpoint, which
            // runs the multi-ellipsoidal propagation cascade once per requested frequency and returns
            // the serialized PropagationResult instances.
            string communicationWebAPIUri = webHostEnvironment.IsDevelopment() ? Constants.Default.CommunicationWebAPIUri_Development : Constants.Default.CommunicationWebAPIUri;

            string? json_GeometricalPropagationModel = Core.Convert.ToSystem_String(geometricalPropagationModel);
            if (string.IsNullOrWhiteSpace(json_GeometricalPropagationModel))
            {
                return NoContent();
            }

            StringBuilder stringBuilder = new($"{communicationWebAPIUri}/communication/geometricalpropagationmodel/propagationresults");
            stringBuilder.Append($"?polarization={Uri.EscapeDataString(polarization)}");
            stringBuilder.Append($"&relativePermittivity={relativePermittivity.ToString(CultureInfo.InvariantCulture)}");
            stringBuilder.Append($"&conductivity={conductivity.ToString(CultureInfo.InvariantCulture)}");
            foreach (double frequency in frequencies)
            {
                stringBuilder.Append($"&frequency={frequency.ToString(CultureInfo.InvariantCulture)}");
            }

            using StringContent stringContent = new(json_GeometricalPropagationModel, Encoding.UTF8, "application/json");

            httpResponseMessage = await httpClient.PostAsync(stringBuilder.ToString(), stringContent, cancellationToken);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            json = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(json))
            {
                return NoContent();
            }

            #endregion DiGi.Communication.WebAPI call

            #region Render payload

            if (System.Text.Json.Nodes.JsonNode.Parse(json) is not System.Text.Json.Nodes.JsonArray jsonArray)
            {
                return NoContent();
            }

            // AI-NOTE (multi-frequency extensibility): one entry per calculated frequency. The 3D
            // view currently renders the first entry; a per frequency visibility toggle only needs to
            // iterate this array (see communication-tools.js renderResults).
            List<object> results = [];
            foreach (System.Text.Json.Nodes.JsonNode? jsonNode in jsonArray)
            {
                if (jsonNode is not System.Text.Json.Nodes.JsonObject jsonObject_Result)
                {
                    continue;
                }

                double frequency = jsonObject_Result["Frequency"]?.GetValue<double>() ?? double.NaN;

                Communication.Propagation.Classes.PropagationResult? propagationResult = Core.Create.SerializableObject<Communication.Propagation.Classes.PropagationResult>(jsonObject_Result["PropagationResult"] as System.Text.Json.Nodes.JsonObject);
                if (double.IsNaN(frequency) || propagationResult is null)
                {
                    continue;
                }

                List<Communication.Propagation.Classes.EllipsoidComponent>? ellipsoidComponents = propagationResult.EllipsoidComponents;
                List<Communication.Propagation.Classes.Ray>? rays = propagationResult.Rays;

                // Ray arrival directions are expressed in the model coordinate system (origin at the
                // receiver for the angles theta/phi); they are converted to world direction vectors
                // here so the 3D view only deals with world coordinates. The component walk mirrors
                // the ray generation order of DiGi.Communication.Propagation Create.PropagationResult
                // (components with non-positive power contribute no rays), which associates each ray
                // with the delay of its propagation ellipsoid.
                List<object> rayPayloads = [];
                if (ellipsoidComponents is not null && rays is not null)
                {
                    int rayIndex = 0;
                    foreach (Communication.Propagation.Classes.EllipsoidComponent ellipsoidComponent in ellipsoidComponents)
                    {
                        if (ellipsoidComponent.Power <= 0 || ellipsoidComponent.ReferencePower <= 0)
                        {
                            continue;
                        }

                        int rayContributionCount = ellipsoidComponent.RayContributions?.Count ?? 0;
                        for (int i = 0; i < rayContributionCount && rayIndex < rays.Count; i++, rayIndex++)
                        {
                            Communication.Propagation.Classes.Ray ray = rays[rayIndex];

                            double x_Model = Math.Sin(ray.Theta) * Math.Cos(ray.Phi);
                            double y_Model = Math.Sin(ray.Theta) * Math.Sin(ray.Phi);
                            double z_Model = Math.Cos(ray.Theta);

                            rayPayloads.Add(new
                            {
                                direction = new
                                {
                                    x = (x_Model * vector3D_AxisX.X) + (y_Model * vector3D_AxisY.X) + (z_Model * vector3D_AxisZ.X),
                                    y = (x_Model * vector3D_AxisX.Y) + (y_Model * vector3D_AxisY.Y) + (z_Model * vector3D_AxisZ.Y),
                                    z = (x_Model * vector3D_AxisX.Z) + (y_Model * vector3D_AxisY.Z) + (z_Model * vector3D_AxisZ.Z)
                                },
                                theta = ray.Theta,
                                phi = ray.Phi,
                                power = ray.Power,
                                delay = ellipsoidComponent.Delay
                            });
                        }
                    }
                }

                // The 3D view renders one propagation ellipsoid: the component carrying the highest
                // measured fractional power (the dominant propagation path). All components are still
                // returned for the results panel.
                Communication.Propagation.Classes.EllipsoidComponent? ellipsoidComponent_Dominant = null;
                if (ellipsoidComponents is not null)
                {
                    foreach (Communication.Propagation.Classes.EllipsoidComponent ellipsoidComponent in ellipsoidComponents)
                    {
                        if (ellipsoidComponent.SemiMinorAxis <= 0)
                        {
                            continue;
                        }

                        if (ellipsoidComponent_Dominant is null || ellipsoidComponent.MeasuredFractionalPower > ellipsoidComponent_Dominant.MeasuredFractionalPower)
                        {
                            ellipsoidComponent_Dominant = ellipsoidComponent;
                        }
                    }
                }

                Point3D? point3D_Center = location_Transmitter.Mid(location_Receiver);

                object? ellipsoidPayload = ellipsoidComponent_Dominant is null || point3D_Center is null ? null : new
                {
                    center = new { x = point3D_Center.X, y = point3D_Center.Y, z = point3D_Center.Z },
                    axis = new { x = vector3D_AxisX.X, y = vector3D_AxisX.Y, z = vector3D_AxisX.Z },
                    semiMajorAxis = ellipsoidComponent_Dominant.SemiMajorAxis,
                    semiMinorAxis = ellipsoidComponent_Dominant.SemiMinorAxis,
                    delay = ellipsoidComponent_Dominant.Delay,
                    measuredFractionalPower = ellipsoidComponent_Dominant.MeasuredFractionalPower
                };

                List<object> componentPayloads = [];
                if (ellipsoidComponents is not null)
                {
                    foreach (Communication.Propagation.Classes.EllipsoidComponent ellipsoidComponent in ellipsoidComponents)
                    {
                        componentPayloads.Add(new
                        {
                            delay = ellipsoidComponent.Delay,
                            power = ellipsoidComponent.Power,
                            fractionalPower = ellipsoidComponent.FractionalPower,
                            measuredFractionalPower = ellipsoidComponent.MeasuredFractionalPower
                        });
                    }
                }

                results.Add(new
                {
                    frequency,
                    polarization,
                    relativePermittivity,
                    conductivity,
                    totalPower = propagationResult.TotalPower,
                    directionalPower = propagationResult.DirectionalPower,
                    rays = rayPayloads,
                    ellipsoid = ellipsoidPayload,
                    ellipsoidComponents = componentPayloads
                });
            }

            if (results.Count == 0)
            {
                return NoContent();
            }

            #endregion Render payload

            return Json(new
            {
                distance,
                transmitter = new { x = location_Transmitter.X, y = location_Transmitter.Y, z = location_Transmitter.Z },
                receiver = new { x = location_Receiver.X, y = location_Receiver.Y, z = location_Receiver.Z },
                results
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
