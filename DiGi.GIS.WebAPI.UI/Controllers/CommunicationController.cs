using DiGi.Analytical.Building.Classes;
using DiGi.Communication.Classes;
using DiGi.Communication.Enums;
using DiGi.Communication.Interfaces;
using DiGi.Core.Constants;
using DiGi.Geometry.Spatial;
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

            List<ScatteringObject>? scatteringObjects = building2Ds.ToCommunication(communicationCalculationParameter.StoreyHeight ?? Constants.Default.StoreyHeight);

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

            // The selection below mirrors DiGi.Communication
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
            // fact (DiGi.Communication.xUnit Facts.ToPropagation_PropagationModel_TypicalUrban):
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

                Communication.Classes.PropagationResult? propagationResult = Core.Create.SerializableObject<Communication.Classes.PropagationResult>(jsonObject_Result["PropagationResult"] as System.Text.Json.Nodes.JsonObject);
                if (double.IsNaN(frequency) || propagationResult is null)
                {
                    continue;
                }

                List<Communication.Classes.EllipsoidComponent>? ellipsoidComponents = propagationResult.EllipsoidComponents;
                List<Communication.Classes.ArrivalRay>? rays = propagationResult.Rays;

                // Ray arrival directions are expressed in the model coordinate system (origin at the
                // receiver for the angles theta/phi); they are converted to world direction vectors
                // here so the 3D view only deals with world coordinates. The component walk mirrors
                // the ray generation order of DiGi.Communication Create.PropagationResult
                // (components with non-positive power contribute no rays), which associates each ray
                // with the delay of its propagation ellipsoid.
                List<object> rayPayloads = [];
                if (ellipsoidComponents is not null && rays is not null)
                {
                    int rayIndex = 0;
                    foreach (Communication.Classes.EllipsoidComponent ellipsoidComponent in ellipsoidComponents)
                    {
                        if (ellipsoidComponent.Power <= 0 || ellipsoidComponent.ReferencePower <= 0)
                        {
                            continue;
                        }

                        int rayContributionCount = ellipsoidComponent.RayContributions?.Count ?? 0;
                        for (int i = 0; i < rayContributionCount && rayIndex < rays.Count; i++, rayIndex++)
                        {
                            Communication.Classes.ArrivalRay ray = rays[rayIndex];

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
                Communication.Classes.EllipsoidComponent? ellipsoidComponent_Dominant = null;
                if (ellipsoidComponents is not null)
                {
                    foreach (Communication.Classes.EllipsoidComponent ellipsoidComponent in ellipsoidComponents)
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
                    foreach (Communication.Classes.EllipsoidComponent ellipsoidComponent in ellipsoidComponents)
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

            string gLBUrl = $"~/buildingmodel/glb/buildingsbyradius?centerX={centerX.ToString(CultureInfo.InvariantCulture)}&centerY={centerY.ToString(CultureInfo.InvariantCulture)}&radius={radius.ToString(CultureInfo.InvariantCulture)}";
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

        #region [TEMPORARY A/B TESTING]

        // ============================================================================================
        // [TEMPORARY A/B TESTING]
        // Temporary version switch used to compare the current calculation ("V1") against the new
        // implementation ("V2"). The Start view prompt routes the user to v1/buildingsbyradius or
        // v2/buildingsbyradius; each renders the scene wired (via CommunicationSceneViewModel.CalculateUrl)
        // to the matching v1/calculate or v2/calculate endpoint below.
        //
        // Each version is a full, self-contained copy so the losing one can be deleted wholesale.
        // The original GetBuildingsByRadius / CalculateAsync / calculate route above are left untouched
        // as the standard-routing restore target.
        //
        // TO REMOVE (restore standard routing): delete this whole region, then revert the tagged lines
        // in Start.cshtml, CommunicationSceneView.cshtml and CommunicationSceneViewModel.cs.
        // ============================================================================================

        /// <summary>
        /// [TEMPORARY A/B TESTING] Renders the communication 3D scene view (version 1) wired to the v1 calculation endpoint.
        /// </summary>
        /// <param name="centerX">The X coordinate of the center of the analyzed circular area.</param>
        /// <param name="centerY">The Y coordinate of the center of the analyzed circular area.</param>
        /// <param name="radius">The radius of the analyzed circular area in meters.</param>
        /// <param name="storeyHeight">The optional storey height in meters used for the building extrusions.</param>
        /// <returns>An <see cref="IActionResult"/> rendering the communication scene view for version 1.</returns>
        [HttpGet("v1/buildingsbyradius")]
        public IActionResult GetBuildingsByRadiusV1(
            [FromQuery(Name = "centerX")] double centerX,
            [FromQuery(Name = "centerY")] double centerY,
            [FromQuery(Name = "radius")] double radius,
            [FromQuery(Name = "storeyheight")] double? storeyHeight = null)
        {
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || double.IsNaN(radius) || radius <= 0)
            {
                return BadRequest();
            }

            string gLBUrl = $"~/buildingmodel/glb/buildingsbyradius?centerX={centerX.ToString(CultureInfo.InvariantCulture)}&centerY={centerY.ToString(CultureInfo.InvariantCulture)}&radius={radius.ToString(CultureInfo.InvariantCulture)}";
            if (storeyHeight is not null && storeyHeight.HasValue)
            {
                gLBUrl += $"&storeyheight={storeyHeight.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            string title = $"Buildings V1 ({centerX}, {centerY}) r = {radius} m";

            CommunicationSceneViewModel communicationSceneViewModel = new(title, gLBUrl, centerX, centerY, radius, storeyHeight ?? Constants.Default.StoreyHeight)
            {
                CalculateUrl = "~/communication/v1/calculate"
            };

            return View("CommunicationSceneView", communicationSceneViewModel);
        }

        /// <summary>
        /// [TEMPORARY A/B TESTING] Renders the communication 3D scene view (version 2) wired to the v2 calculation endpoint.
        /// </summary>
        /// <param name="centerX">The X coordinate of the center of the analyzed circular area.</param>
        /// <param name="centerY">The Y coordinate of the center of the analyzed circular area.</param>
        /// <param name="radius">The radius of the analyzed circular area in meters.</param>
        /// <param name="storeyHeight">The optional storey height in meters used for the building extrusions.</param>
        /// <returns>An <see cref="IActionResult"/> rendering the communication scene view for version 2.</returns>
        [HttpGet("v2/buildingsbyradius")]
        public IActionResult GetBuildingsByRadiusV2(
            [FromQuery(Name = "centerX")] double centerX,
            [FromQuery(Name = "centerY")] double centerY,
            [FromQuery(Name = "radius")] double radius,
            [FromQuery(Name = "storeyheight")] double? storeyHeight = null)
        {
            if (double.IsNaN(centerX) || double.IsNaN(centerY) || double.IsNaN(radius) || radius <= 0)
            {
                return BadRequest();
            }

            string gLBUrl = $"~/buildingmodel/glb/buildingsbyradius?centerX={centerX.ToString(CultureInfo.InvariantCulture)}&centerY={centerY.ToString(CultureInfo.InvariantCulture)}&radius={radius.ToString(CultureInfo.InvariantCulture)}";
            if (storeyHeight is not null && storeyHeight.HasValue)
            {
                gLBUrl += $"&storeyheight={storeyHeight.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            string title = $"Buildings V2 ({centerX}, {centerY}) r = {radius} m";

            CommunicationSceneViewModel communicationSceneViewModel = new(title, gLBUrl, centerX, centerY, radius, storeyHeight ?? Constants.Default.StoreyHeight)
            {
                CalculateUrl = "~/communication/v2/calculate"
            };

            return View("CommunicationSceneView", communicationSceneViewModel);
        }

        /// <summary>
        /// [TEMPORARY A/B TESTING] Version 1 (current implementation) of the communication calculation.
        /// <para>The buildings of the analyzed area are fetched as <see cref="BuildingModel"/> instances and converted to <see cref="ScatteringObject"/> instances, packaged together with the antennas into a <see cref="GeometricalPropagationModel"/> and solved in process (<see cref="ScatteringSolver"/> + <see cref="AngularPowerDistributionSolver"/>); nothing is persisted.</para>
        /// </summary>
        /// <param name="communicationCalculationParameter">The analyzed circular area and the antennas placed by the user.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the calculation result JSON grouped by delay (ascending): the propagation ellipsoids, the scattering polylines (one per <see cref="ScatteringPointGroup"/>) and the angular power distribution vectors, all in world coordinates.</returns>
        [HttpPost("v1/calculate")]
        public async Task<IActionResult> CalculateAsyncV1([FromBody] CommunicationCalculationParameter? communicationCalculationParameter, CancellationToken cancellationToken = default)
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
            UrlBuilder urlBuilder = new("https://api.digiproject.uk/gis/buildingmodel/itemsbycircle");
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

            List<BuildingModel>? buildingModels = Core.Convert.ToDiGi<BuildingModel>(json);

            List<ScatteringObject>? scatteringObjects = buildingModels.ToCommunication();

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

            DefaultSimpleMultipathPowerDelayProfile profile = DefaultSimpleMultipathPowerDelayProfile.TypicalUrban;
            if (!string.IsNullOrWhiteSpace(communicationCalculationParameter.DefaultSimpleMultipathPowerDelayProfile))
            {
                _ = Enum.TryParse(communicationCalculationParameter.DefaultSimpleMultipathPowerDelayProfile, true, out profile);
            }

            SimpleMultipathPowerDelayProfile? simpleMultipathPowerDelayProfile = Communication.Create.SimpleMultipathPowerDelayProfile(profile);

            geometricalPropagationModel.Assign(simpleMultipathPowerDelayProfile, antennas[0], antennas[1]);

            double minElevation = double.MaxValue;
            if (scatteringObjects is not null)
            {
                foreach (ScatteringObject scatteringObject in scatteringObjects)
                {
                    geometricalPropagationModel.Update(scatteringObject);

                    if(scatteringObject?.Mesh3D?.GetBoundingBox()?.Min.Z is double elevation && minElevation > elevation)
                    {
                        minElevation = elevation;
                    }
                }
            }

            if(minElevation == double.MaxValue)
            {
                minElevation = 0;
            }

            #endregion GeometricalPropagationModel

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
                return BadRequest();
            }

            double distance = location_Transmitter.Distance(location_Receiver);
            if (distance <= 0)
            {
                return BadRequest();
            }

            #endregion Transmitter/receiver selection

            #region Solvers

            ScatteringSolver scatteringSolver = new()
            {
                GeometricalPropagationModel = geometricalPropagationModel,
                ScatteringSolverOptions = new ScatteringSolverOptions(Communication.Constants.Factor.Angle, 0.1, Tolerance.Distance)
            };

            scatteringSolver.Solve();

            AngularPowerDistributionSolver angularPowerDistributionSolver = new()
            {
                GeometricalPropagationModel = geometricalPropagationModel,
                AngularPowerDistributionSolverOptions = new AngularPowerDistributionSolverOptions()
            };

            angularPowerDistributionSolver.Solve();

            #endregion Solvers

            #region Render payload

            // Everything below is expressed in world coordinates only, grouped by delay: the 3D
            // view drives a delay slider (ascending, General panel) and renders, for the selected
            // delay, the propagation ellipsoid(s), the scattering polylines (one per
            // ScatteringPointGroup, with the profile locations required for the auxiliary
            // polylines) and the angular power distribution vectors (scaled client side by the
            // user provided factor). See renderDelayResults in communication-tools.js.
            static object PointPayload(Point3D point3D) => new { x = point3D.X, y = point3D.Y, z = point3D.Z };
            static object VectorPayload(Vector3D vector3D) => new { x = vector3D.X, y = vector3D.Y, z = vector3D.Z };

            static void Add(Dictionary<double, List<object>> payloads, double delay, object payload)
            {
                if (!payloads.TryGetValue(delay, out List<object>? values))
                {
                    values = [];
                    payloads[delay] = values;
                }

                values.Add(payload);
            }

            // All available delays, ascending (the delay slider order in the General panel).
            SortedSet<double> delays = [];

            // Payload fragments keyed by delay.
            Dictionary<double, List<object>> ellipsoidPayloads = [];
            Dictionary<double, List<object>> polylinePayloads = [];
            Dictionary<double, List<object>> vectorGroupPayloads = [];

            IEnumerable<ScatteringProfile>? scatteringProfiles = geometricalPropagationModel.GetScatteringProfiles<ScatteringProfile>();
            if (scatteringProfiles is not null)
            {
                foreach (ScatteringProfile scatteringProfile in scatteringProfiles)
                {
                    if (scatteringProfile?.Scatterings is not IEnumerable<Scattering> scatterings)
                    {
                        continue;
                    }

                    Point3D? location_1 = scatteringProfile.Location_1;
                    Point3D? location_2 = scatteringProfile.Location_2;
                    if (location_1 is null || location_2 is null)
                    {
                        continue;
                    }

                    object payload_Location_1 = PointPayload(location_1);
                    object payload_Location_2 = PointPayload(location_2);

                    foreach (Scattering scattering in scatterings)
                    {
                        // The delay for a given scattering is the same for all points in its
                        // scattering point groups.
                        double delay = scattering.Delay;

                        delays.Add(delay);

                        // The propagation ellipsoid for the given delay, meshed and cut by the
                        // horizontal plane at the lowest scattering object elevation: only the part
                        // above the ground plane is rendered by the 3D view, so the payload carries
                        // the triangulated world coordinate mesh (flat vertex/index arrays) instead
                        // of the analytic ellipsoid parameters.
                        Ellipsoid? ellipsoid = Communication.Create.Ellipsoid(location_1, location_2, delay);

                        if (ellipsoid?.Center is Point3D point3D_Center && ellipsoid.DirectionA is Vector3D vector3D_Axis)
                        {
                            Mesh3D? mesh3D = ellipsoid.Mesh3D(Communication.Constants.Factor.Angle);

                            List<Mesh3D> mesh3Ds = [];
                            if (Geometry.Spatial.Query.TrySplit(Geometry.Spatial.Create.Plane(minElevation), mesh3D, out List<Mesh3D>? mesh3Ds_Above, out List<Mesh3D>? _) && mesh3Ds_Above is not null && mesh3Ds_Above.Count > 0)
                            {
                                mesh3Ds.AddRange(mesh3Ds_Above);
                            }
                            else if (mesh3D is not null)
                            {
                                // No split available (e.g. the ellipsoid does not cross the plane):
                                // fall back to the full ellipsoid mesh.
                                mesh3Ds.Add(mesh3D);
                            }

                            List<double> vertices = [];
                            List<int> indices = [];
                            foreach (Mesh3D mesh3D_Temp in mesh3Ds)
                            {
                                List<Point3D>? point3Ds = mesh3D_Temp?.GetPoints();
                                List<int[]>? indexes = mesh3D_Temp?.GetIndexes();
                                if (point3Ds is null || indexes is null)
                                {
                                    continue;
                                }

                                int indexOffset = vertices.Count / 3;
                                foreach (Point3D point3D in point3Ds)
                                {
                                    vertices.Add(point3D.X);
                                    vertices.Add(point3D.Y);
                                    vertices.Add(point3D.Z);
                                }

                                foreach (int[] triangle in indexes)
                                {
                                    if (triangle is null || triangle.Length < 3)
                                    {
                                        continue;
                                    }

                                    indices.Add(triangle[0] + indexOffset);
                                    indices.Add(triangle[1] + indexOffset);
                                    indices.Add(triangle[2] + indexOffset);
                                }
                            }

                            Add(ellipsoidPayloads, delay, new
                            {
                                center = PointPayload(point3D_Center),
                                axis = VectorPayload(vector3D_Axis),
                                semiMajorAxis = ellipsoid.A,
                                semiMinorAxis = ellipsoid.B,
                                mesh = vertices.Count == 0 || indices.Count == 0 ? null : new { vertices, indices }
                            });
                        }

                        if (scattering.ScatteringPointGroups is not IEnumerable<ScatteringPointGroup> scatteringPointGroups)
                        {
                            continue;
                        }

                        foreach (ScatteringPointGroup scatteringPointGroup in scatteringPointGroups)
                        {
                            if (scatteringPointGroup?.Points is not List<Point3D> point3Ds || point3Ds.Count == 0)
                            {
                                continue;
                            }

                            // One polyline per ScatteringPointGroup (reference identifies the
                            // component the group was created for); the profile locations enable
                            // the auxiliary polylines (location_1 -> point -> location_2) shown
                            // when the polyline is selected in the 3D view.
                            Add(polylinePayloads, delay, new
                            {
                                reference = scatteringPointGroup.Reference,
                                location1 = payload_Location_1,
                                location2 = payload_Location_2,
                                points = point3Ds.ConvertAll(PointPayload)
                            });
                        }
                    }
                }
            }

            IEnumerable<AngularPowerDistributionProfile>? angularPowerDistributionProfiles = geometricalPropagationModel.GetAngularPowerDistributionProfiles<AngularPowerDistributionProfile>();
            if (angularPowerDistributionProfiles is not null)
            {
                foreach (AngularPowerDistributionProfile angularPowerDistributionProfile in angularPowerDistributionProfiles)
                {
                    if (angularPowerDistributionProfile.Location is not Point3D location || angularPowerDistributionProfile.AngularPowerDistributions is not IEnumerable<AngularPowerDistribution> angularPowerDistributions)
                    {
                        continue;
                    }

                    object payload_Location = PointPayload(location);

                    foreach (AngularPowerDistribution angularPowerDistribution in angularPowerDistributions)
                    {
                        // The delay for an angular power distribution is the same for all vectors
                        // in the distribution.
                        double delay = angularPowerDistribution.Delay;

                        delays.Add(delay);

                        // The vectors visualized at the location; their length carries the power,
                        // so they are sent unnormalized and scaled client side only.
                        List<Vector3D>? vector3Ds = angularPowerDistribution.Vectors;
                        if (vector3Ds is null || vector3Ds.Count == 0)
                        {
                            continue;
                        }

                        Add(vectorGroupPayloads, delay, new
                        {
                            location = payload_Location,
                            vectors = vector3Ds.ConvertAll(VectorPayload)
                        });
                    }
                }
            }

            List<object> results = [];
            foreach (double delay in delays)
            {
                results.Add(new
                {
                    delay,
                    ellipsoids = ellipsoidPayloads.GetValueOrDefault(delay) ?? [],
                    polylines = polylinePayloads.GetValueOrDefault(delay) ?? [],
                    vectorGroups = vectorGroupPayloads.GetValueOrDefault(delay) ?? []
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
                // The delays array (ascending, one entry per results entry) discriminates this V1
                // payload from the V2 one in communication-tools.js and feeds the delay slider.
                delays = delays.ToList(),
                results
            });
        }

        /// <summary>
        /// [TEMPORARY A/B TESTING] Version 2 (new implementation) of the communication calculation. Starts as a full self-contained copy of <see cref="CalculateAsync"/>; replace the body below with the new V2 algorithm.
        /// </summary>
        /// <param name="communicationCalculationParameter">The analyzed circular area and the antennas placed by the user.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the calculation result JSON.</returns>
        [HttpPost("v2/calculate")]
        public async Task<IActionResult> CalculateAsyncV2([FromBody] CommunicationCalculationParameter? communicationCalculationParameter, CancellationToken cancellationToken = default)
        {
            // [TEMPORARY A/B TESTING] Replace the body below with the new V2 implementation. It is currently an exact clone of CalculateAsyncV1 so the two versions return identical results until the new code is dropped in.
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

            List<ScatteringObject>? scatteringObjects = building2Ds.ToCommunication(communicationCalculationParameter.StoreyHeight ?? Constants.Default.StoreyHeight);

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

            // The selection below mirrors DiGi.Communication
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
            // fact (DiGi.Communication.xUnit Facts.ToPropagation_PropagationModel_TypicalUrban):
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

                Communication.Classes.PropagationResult? propagationResult = Core.Create.SerializableObject<Communication.Classes.PropagationResult>(jsonObject_Result["PropagationResult"] as System.Text.Json.Nodes.JsonObject);
                if (double.IsNaN(frequency) || propagationResult is null)
                {
                    continue;
                }

                List<Communication.Classes.EllipsoidComponent>? ellipsoidComponents = propagationResult.EllipsoidComponents;
                List<Communication.Classes.ArrivalRay>? rays = propagationResult.Rays;

                // Ray arrival directions are expressed in the model coordinate system (origin at the
                // receiver for the angles theta/phi); they are converted to world direction vectors
                // here so the 3D view only deals with world coordinates. The component walk mirrors
                // the ray generation order of DiGi.Communication Create.PropagationResult
                // (components with non-positive power contribute no rays), which associates each ray
                // with the delay of its propagation ellipsoid.
                List<object> rayPayloads = [];
                if (ellipsoidComponents is not null && rays is not null)
                {
                    int rayIndex = 0;
                    foreach (Communication.Classes.EllipsoidComponent ellipsoidComponent in ellipsoidComponents)
                    {
                        if (ellipsoidComponent.Power <= 0 || ellipsoidComponent.ReferencePower <= 0)
                        {
                            continue;
                        }

                        int rayContributionCount = ellipsoidComponent.RayContributions?.Count ?? 0;
                        for (int i = 0; i < rayContributionCount && rayIndex < rays.Count; i++, rayIndex++)
                        {
                            Communication.Classes.ArrivalRay ray = rays[rayIndex];

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
                Communication.Classes.EllipsoidComponent? ellipsoidComponent_Dominant = null;
                if (ellipsoidComponents is not null)
                {
                    foreach (Communication.Classes.EllipsoidComponent ellipsoidComponent in ellipsoidComponents)
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
                    foreach (Communication.Classes.EllipsoidComponent ellipsoidComponent in ellipsoidComponents)
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

        #endregion [TEMPORARY A/B TESTING]
    }
}
