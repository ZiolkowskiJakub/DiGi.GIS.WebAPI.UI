using DiGi.Analytical.Building.Classes;
using DiGi.Analytical.Building.Solar;
using DiGi.Core.Classes;
using DiGi.EPW;
using DiGi.EPW.Classes;
using DiGi.Geometry.Spatial.Classes;
using DiGi.Geometry.Spatial.Interfaces;
using DiGi.GIS.WebAPI.UI.Classes;
using DiGi.Solar.Classes;
using DiGi.Solar.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Create
    {
        /// <summary>
        /// Calculates the annual solar radiation on the external walls and roofs of a building, shaded by the building itself and by its neighbours, over one EPW year.
        /// <para>Composes <see cref="Query.SolarReceiverNormals(BuildingModel?)"/>, <c>ToSolar</c> (the external walls and roofs receive, everything else - the building's other components and every neighbour - only casts shade) and <see cref="SurfaceSolarRadiationResults(ShadingModel?, IDictionary{string, Vector3D}?, EPWFile?, ShadingSolverOptions?, Action{string}?)"/>. A caller that has to refuse a request between those steps (a receiver or caster limit) calls them itself.</para>
        /// </summary>
        /// <param name="buildingModel">The analysed building model, already stamped with its coordinates and time zone (<c>GIS.Analytical.Modify.UpdateBuildingInformation</c>). This value can be null.</param>
        /// <param name="buildingModels_Surrounding">The neighbouring building models casting shade, or null for none. A copy of <paramref name="buildingModel"/> among them is skipped.</param>
        /// <param name="ePWFile">The EPW weather file. This value can be null.</param>
        /// <param name="shadingSolverOptions">The solver options; its time series is replaced by the EPW hours. Null uses the defaults.</param>
        /// <returns>One result per receiving surface, or <see langword="null"/> when the model is null or unstamped (no coordinates), has no external envelope, or the EPW file has no usable hour.</returns>
        public static List<SurfaceSolarRadiationResult>? SurfaceSolarRadiationResults(this BuildingModel? buildingModel, IEnumerable<BuildingModel>? buildingModels_Surrounding, EPWFile? ePWFile, ShadingSolverOptions? shadingSolverOptions)
        {
            if (buildingModel is null)
            {
                return null;
            }

            Dictionary<string, Vector3D>? normals = buildingModel.SolarReceiverNormals();
            if (normals is null || normals.Count == 0)
            {
                return null;
            }

            ShadingModel? shadingModel = buildingModel.ToSolar(buildingModels_Surrounding, x => new GuidReference(x).ToString() is string reference && normals.ContainsKey(reference));

            return shadingModel.SurfaceSolarRadiationResults(normals, ePWFile, shadingSolverOptions);
        }

        /// <summary>
        /// Solves a shading model over the hours of one EPW year and integrates the irradiation of each receiving surface.
        /// <para>Every EPW hour with global, direct and diffuse radiation is sampled at its mid-hour instant in the reference year (<see cref="Query.SolarReferenceDateTime(DateTime)"/>). The shading model is solved for those instants with <see cref="ShadingSolver"/>, which writes its results into <paramref name="shadingModel"/>. For each receiver and hour, the irradiance of the surface is <c>Solar.Create.IrradianceResult</c> on its outward normal, and the power is <c>Solar.Create.SolarPowerResult_ByShadingFactor</c> with the solved shading factor: the shadow blocks the beam component only. An unshaded twin with factor 0 gives <see cref="SurfaceSolarRadiationResult.IrradiationUnshaded"/>.</para>
        /// <para>Each receiver's results are read out once into a map of shaded area by instant; <c>ShadingModel.TryGetShadingFactor</c> would fetch and scan all of them on every call, 9–75 ms per call on the web UI host (DiGi.GIS.WebAPI.UI#59, comment 5830021444). The sun direction and the albedo depend only on the hour and are computed once per hour.</para>
        /// <para>Snow cover is never assumed: the served EPW files carry either filler snow depth (IWEC WARSAW reports snow for 8 322 hours) or no albedo at all, so the albedo is the file's own value or the 0.2 default.</para>
        /// <para>A receiver without an outward normal in <paramref name="normals"/>, with no area, or that the solver could not assign (no plane or no triangulation) gets no result. A daytime hour missing from a receiver's results is skipped.</para>
        /// </summary>
        /// <param name="shadingModel">The shading model: the receivers and shading-only casters, with coordinates and a time zone. It receives the solver results. This value can be null.</param>
        /// <param name="normals">The outward unit normal of each receiver, keyed by its reference (<see cref="Query.SolarReceiverNormals(BuildingModel?)"/>). This value can be null.</param>
        /// <param name="ePWFile">The EPW weather file. This value can be null.</param>
        /// <param name="shadingSolverOptions">The solver options; a copy is solved with its time series replaced by the EPW hours, so the caller's instance is left untouched. Null uses the defaults.</param>
        /// <param name="log">Receives one line with the direction groups and the solve and aggregation times, or null for none.</param>
        /// <returns>One result per receiving surface, or <see langword="null"/> when an input is null, the model has no coordinates, the EPW file has no usable hour, or the solver fails.</returns>
        public static List<SurfaceSolarRadiationResult>? SurfaceSolarRadiationResults(this ShadingModel? shadingModel, IDictionary<string, Vector3D>? normals, EPWFile? ePWFile, ShadingSolverOptions? shadingSolverOptions, Action<string>? log = null)
        {
            // (0, 0) is the default of an unstamped BuildingInformation, not a location in Poland.
            Coordinates? coordinates = shadingModel?.Coordinates;
            if (shadingModel is null || coordinates is null || (coordinates.Latitude == 0 && coordinates.Longitude == 0) || normals is null || ePWFile is null)
            {
                return null;
            }

            IList<DataRecord>? dataRecords = ePWFile.DataRecords;
            if (dataRecords is null || dataRecords.Count == 0)
            {
                return null;
            }

            List<DateTime> dateTimes = [];
            List<double> globalHorizontalRadiations = [];
            List<double> directNormalRadiations = [];
            List<double> diffuseHorizontalRadiations = [];
            List<double> albedos = [];

            HashSet<DateTime> dateTimes_Unique = [];
            foreach (DataRecord dataRecord in dataRecords)
            {
                if (dataRecord is null)
                {
                    continue;
                }

                float? globalHorizontalRadiation = dataRecord.GlobalHorizontalRadiationValue();
                float? directNormalRadiation = dataRecord.DirectNormalRadiationValue();
                float? diffuseHorizontalRadiation = dataRecord.DiffuseHorizontalRadiationValue();
                if (globalHorizontalRadiation is null || directNormalRadiation is null || diffuseHorizontalRadiation is null)
                {
                    continue;
                }

                DateTime? dateTime = dataRecord.DateTime.SolarReferenceDateTime();
                if (dateTime is null || !dateTimes_Unique.Add(dateTime.Value))
                {
                    continue;
                }

                dateTimes.Add(dateTime.Value);
                globalHorizontalRadiations.Add(globalHorizontalRadiation.Value);
                directNormalRadiations.Add(directNormalRadiation.Value);
                diffuseHorizontalRadiations.Add(diffuseHorizontalRadiation.Value);
                albedos.Add(Solar.Query.Albedo(dataRecord.AlbedoValue(), false));
            }

            int count = dateTimes.Count;
            if (count == 0)
            {
                return null;
            }

            ShadingSolverOptions shadingSolverOptions_Temp = shadingSolverOptions is null ? new() : new(shadingSolverOptions);
            shadingSolverOptions_Temp.TimeSeries = new DateTimeCollection(dateTimes);

            Stopwatch stopwatch = Stopwatch.StartNew();

            ShadingSolver shadingSolver = new(shadingModel, shadingSolverOptions_Temp);
            if (!shadingSolver.Solve())
            {
                return null;
            }

            long milliseconds_Solve = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();

            // The sun depends only on the hour. The night direction is kept too: the irradiance of a
            // night hour still needs one, and its beam is zeroed by passing no direct radiation.
            Vector3D?[] sunDirections = new Vector3D?[count];
            bool[] sunUps = new bool[count];
            Dictionary<DateTime, Vector3D> sunDirections_Day = [];
            for (int i = 0; i < count; i++)
            {
                sunDirections[i] = Solar.Query.SunDirection(shadingModel, dateTimes[i], true);

                Vector3D? sunDirection_Day = Solar.Query.SunDirection(shadingModel, dateTimes[i], false);
                sunUps[i] = sunDirection_Day is not null;
                if (sunDirection_Day is not null)
                {
                    sunDirections_Day[dateTimes[i]] = sunDirection_Day;
                }
            }

            List<SurfaceSolarRadiationResult> result = [];

            List<IShadingElement>? shadingElements = shadingModel.GetShadingElements<IShadingElement>(false);
            if (shadingElements is not null)
            {
                foreach (IShadingElement shadingElement in shadingElements)
                {
                    string? reference = shadingElement?.Reference;
                    if (reference is null || !normals.TryGetValue(reference, out Vector3D? normal) || normal is null)
                    {
                        continue;
                    }

                    IPolygonalFace3D? polygonalFace3D = shadingElement!.PolygonalFace3D;
                    double area = polygonalFace3D is null ? double.NaN : polygonalFace3D.GetArea();
                    if (!double.IsFinite(area) || area <= 0)
                    {
                        continue;
                    }

                    List<IShadingSolverResult>? shadingSolverResults = shadingModel.GetShadingSolverResults<IShadingSolverResult>(shadingElement);
                    if (shadingSolverResults is null)
                    {
                        continue;
                    }

                    Dictionary<DateTime, double> areas_Shaded = [];
                    foreach (IShadingSolverResult shadingSolverResult in shadingSolverResults)
                    {
                        if (shadingSolverResult is not null)
                        {
                            areas_Shaded[shadingSolverResult.DateTime] = shadingSolverResult.Area;
                        }
                    }

                    // Energies in Wh: each EPW value is a mean power over one hour.
                    double beam = 0;
                    double diffuse = 0;
                    double ground = 0;
                    double unshaded = 0;

                    for (int i = 0; i < count; i++)
                    {
                        double shadingFactor = 0;
                        if (sunUps[i])
                        {
                            if (!areas_Shaded.TryGetValue(dateTimes[i], out double area_Shaded))
                            {
                                continue;
                            }

                            shadingFactor = Math.Clamp(area_Shaded / area, 0, 1);
                        }

                        IrradianceResult? irradianceResult = Solar.Create.IrradianceResult(normal, sunDirections[i], globalHorizontalRadiations[i], sunUps[i] ? directNormalRadiations[i] : 0, diffuseHorizontalRadiations[i], albedos[i]);
                        if (irradianceResult is null)
                        {
                            continue;
                        }

                        SolarPowerResult? solarPowerResult = Solar.Create.SolarPowerResult_ByShadingFactor(irradianceResult, area, shadingFactor);
                        SolarPowerResult? solarPowerResult_Unshaded = Solar.Create.SolarPowerResult_ByShadingFactor(irradianceResult, area, 0);
                        if (solarPowerResult is null || solarPowerResult_Unshaded is null)
                        {
                            continue;
                        }

                        // The split of SolarPowerResult.Power: the beam reaches the unshaded area only. The
                        // local irradiance result is used because the property clones on every access.
                        beam += solarPowerResult.UnshadedArea * irradianceResult.Beam;
                        diffuse += solarPowerResult.TotalArea * irradianceResult.Diffuse;
                        ground += solarPowerResult.TotalArea * irradianceResult.Ground;
                        unshaded += solarPowerResult_Unshaded.Power;
                    }

                    double energy = (beam + diffuse + ground) / 1000;

                    result.Add(new SurfaceSolarRadiationResult(
                        reference,
                        area,
                        energy / area,
                        beam / 1000 / area,
                        diffuse / 1000 / area,
                        ground / 1000 / area,
                        unshaded / 1000 / area,
                        energy));
                }
            }

            if (log is not null)
            {
                int count_Group = Solar.Query.GroupDirections(sunDirections_Day, shadingSolverOptions_Temp.AngleTolerance)?.Count ?? 0;
                log(string.Format(CultureInfo.InvariantCulture, "Solar radiation: {0} hours, {1} direction groups, solve {2} ms, aggregation {3} ms.", count, count_Group, milliseconds_Solve, stopwatch.ElapsedMilliseconds));
            }

            return result;
        }
    }
}
