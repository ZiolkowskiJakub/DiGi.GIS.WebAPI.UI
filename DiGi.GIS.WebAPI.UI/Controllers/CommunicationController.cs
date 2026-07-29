using DiGi.Analytical.Building.Classes;
using DiGi.Communication.Classes;
using DiGi.Communication.Enums;
using DiGi.Communication.Interfaces;
using DiGi.Core.Classes;
using DiGi.Core.Constants;
using DiGi.Geometry.Spatial;
using DiGi.Geometry.Spatial.Classes;
using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.GIS.WebAPI.UI.ViewModels;
using DiGi.WebAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                return BadRequest("The analyzed area center coordinates and radius must be valid positive numbers.");
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

        /// <summary>
        /// Executes the communication calculation for the antennas placed in the 3D view.
        /// <para>The buildings of the analyzed area are fetched as <see cref="BuildingModel"/> instances and converted to <see cref="ScatteringObject"/> instances, packaged together with the antennas into a <see cref="GeometricalPropagationModel"/> and solved in process (<see cref="ScatteringSolver"/> + <see cref="AngularPowerDistributionSolver"/>); nothing is persisted.</para>
        /// </summary>
        /// <param name="communicationCalculationParameter">The analyzed circular area and the antennas placed by the user.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by the caller to cancel the asynchronous operation.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> holding the calculation result JSON grouped by delay (ascending): the propagation ellipsoids, the scattering polylines (one per <see cref="ScatteringPointGroup"/>) and the angular power distribution vectors, all in world coordinates.</returns>
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

                // Angular bins are sent as radians (as everything else in this payload); the matrix
                // form converts them to degrees for display. The mid value addresses the bin itself:
                // GetScatteringHits maps a single angle to one bin, whereas passing the bin bounds to
                // GetValues(Range, Range) can spill into the neighbouring bin.
                static object RangePayload(Range<double> range) => new { min = range.Min, max = range.Max };
                static double RangeMid(Range<double> range) => (range.Min + range.Max) / 2.0;

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
                Dictionary<double, List<object>> angularDistributionPayloads = [];

                // References of the scattering objects actually hit. Only these are described at the top
                // level: a district holds thousands of scattering objects but the scattering hits touch a
                // handful of them, and a reference string is long enough that sending the whole lookup
                // would dominate the payload.
                HashSet<string> scatteringHitReferences = [];

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

                            // The scattering hits behind the vectors, kept in their azimuth/elevation
                            // bins so the Details form of the Results panel can render them as a matrix
                            // and drill down into a single bin. Built before the Vectors guard below so
                            // a distribution without renderable vectors still contributes its matrix.
                            // Only populated bins are described, and only non-empty intersections are
                            // emitted: the two range lists are filtered independently, so their cross
                            // product is overwhelmingly empty.
                            if (angularPowerDistribution.GetAzimuthRanges(true) is IReadOnlyList<Range<double>> azimuthRanges && azimuthRanges.Count != 0
                                && angularPowerDistribution.GetElevationRanges(true) is IReadOnlyList<Range<double>> elevationRanges && elevationRanges.Count != 0)
                            {
                                List<object> cells = [];
                                for (int i = 0; i < azimuthRanges.Count; i++)
                                {
                                    double azimuth = RangeMid(azimuthRanges[i]);
                                    for (int j = 0; j < elevationRanges.Count; j++)
                                    {
                                        if (angularPowerDistribution.GetScatteringHits(azimuth, RangeMid(elevationRanges[j])) is not IReadOnlyList<IScatteringHit> scatteringHits || scatteringHits.Count == 0)
                                        {
                                            continue;
                                        }

                                        List<object> hits = [];
                                        foreach (IScatteringHit scatteringHit in scatteringHits)
                                        {
                                            // IScatteringHit carries no location; the direction is the
                                            // per hit quantity the azimuth/elevation binning is derived
                                            // from, and it is unnormalized (its length carries the power).
                                            if (scatteringHit?.Ray3D?.Direction is not Vector3D vector3D_Direction)
                                            {
                                                continue;
                                            }

                                            if (scatteringHit.Reference is string reference && !string.IsNullOrWhiteSpace(reference))
                                            {
                                                scatteringHitReferences.Add(reference);
                                            }

                                            hits.Add(new { x = vector3D_Direction.X, y = vector3D_Direction.Y, z = vector3D_Direction.Z, reference = scatteringHit.Reference });
                                        }

                                        if (hits.Count == 0)
                                        {
                                            continue;
                                        }

                                        cells.Add(new { azimuthIndex = i, elevationIndex = j, hits });
                                    }
                                }

                                if (cells.Count != 0)
                                {
                                    Add(angularDistributionPayloads, delay, new
                                    {
                                        location = payload_Location,
                                        azimuthRanges = new List<Range<double>>(azimuthRanges).ConvertAll(RangePayload),
                                        elevationRanges = new List<Range<double>>(elevationRanges).ConvertAll(RangePayload),
                                        cells
                                    });
                                }
                            }

                            // The vectors visualized at the location; their length carries the power,
                            // so they are sent unnormalized and scaled client side only.
                            if (angularPowerDistribution.Vectors is not IEnumerable<Vector3D> vector3Ds || !vector3Ds.Any())
                            {
                                continue;
                            }

                            Add(vectorGroupPayloads, delay, new
                            {
                                location = payload_Location,
                                vectors = vector3Ds.ToList().ConvertAll(VectorPayload)
                            });
                        }
                    }
                }

                // Electrical properties of the scattering objects the hits above point at, keyed by that
                // reference. Resolved in one bulk lookup rather than per hit, and restricted to the hit
                // references so an unhit district does not travel to the client.
                Dictionary<string, object> scatteringObjectPayloads = [];
                if (scatteringHitReferences.Count != 0 && Communication.Query.ElectricalPropertiesByReference(geometricalPropagationModel) is Dictionary<string, ElectricalProperties> electricalPropertiesByReference)
                {
                    foreach (string reference in scatteringHitReferences)
                    {
                        if (!electricalPropertiesByReference.TryGetValue(reference, out ElectricalProperties? electricalProperties))
                        {
                            continue;
                        }

                        scatteringObjectPayloads[reference] = new { name = electricalProperties.Name, a = electricalProperties.A, b = electricalProperties.B, c = electricalProperties.C, d = electricalProperties.D };
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
                        vectorGroups = vectorGroupPayloads.GetValueOrDefault(delay) ?? [],
                        angularDistributions = angularDistributionPayloads.GetValueOrDefault(delay) ?? []
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
                    // Electrical properties keyed by scattering object reference; the scattering hits
                    // of results[].angularDistributions[].cells[] carry that reference only.
                    scatteringObjects = scatteringObjectPayloads,
                    results
                });
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