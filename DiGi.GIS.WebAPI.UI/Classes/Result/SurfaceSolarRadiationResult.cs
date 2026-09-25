using DiGi.Core.Classes;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.GIS.WebAPI.UI.Classes
{
    /// <summary>
    /// Represents the annual solar radiation received by one external surface (a wall or a roof) of a building, shaded by the building itself and by its neighbours.
    /// <para>Irradiation values are per square metre of the surface, over one EPW year. The beam component reaches only the unshaded part of the surface. Sky diffuse and ground-reflected radiation (isotropic sky) reach the surface in proportion to the part of its sky and of its ground not blocked by the building itself or by its neighbours (<see cref="SkyVisibility"/>, <see cref="GroundVisibility"/>); blocked parts contribute nothing, reflections between buildings are ignored (ZiolkowskiJakub/DiGi.Solar#15).</para>
    /// <para>Instances are plain carriers of already-computed values. Use <see cref="Create.SurfaceSolarRadiationResults(Solar.Classes.ShadingModel?, System.Collections.Generic.IDictionary{string, Geometry.Spatial.Classes.Vector3D}?, EPW.Classes.EPWFile?, Solar.Classes.ShadingSolverOptions?, System.Action{string}?)"/> to calculate them.</para>
    /// </summary>
    public class SurfaceSolarRadiationResult : SerializableResult
    {
        [JsonInclude, JsonPropertyName(nameof(Area))]
        private readonly double area = 0;

        [JsonInclude, JsonPropertyName(nameof(Beam))]
        private readonly double beam = 0;

        [JsonInclude, JsonPropertyName(nameof(Diffuse))]
        private readonly double diffuse = 0;

        [JsonInclude, JsonPropertyName(nameof(Energy))]
        private readonly double energy = 0;

        [JsonInclude, JsonPropertyName(nameof(Ground))]
        private readonly double ground = 0;

        [JsonInclude, JsonPropertyName(nameof(GroundVisibility))]
        private readonly double groundVisibility = 1;

        [JsonInclude, JsonPropertyName(nameof(Irradiation))]
        private readonly double irradiation = 0;

        [JsonInclude, JsonPropertyName(nameof(IrradiationUnshaded))]
        private readonly double irradiationUnshaded = 0;

        [JsonInclude, JsonPropertyName(nameof(Reference))]
        private readonly string? reference = null;

        [JsonInclude, JsonPropertyName(nameof(SkyVisibility))]
        private readonly double skyVisibility = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceSolarRadiationResult"/> class.
        /// </summary>
        /// <param name="reference">The <see cref="GuidReference"/> string of the building component the surface belongs to.</param>
        /// <param name="area">The area of the surface, in m².</param>
        /// <param name="irradiation">The annual irradiation of the surface with shading, in kWh/m² per year.</param>
        /// <param name="beam">The annual beam (direct) irradiation reaching the unshaded part of the surface, in kWh/m² per year of the whole surface.</param>
        /// <param name="diffuse">The annual sky diffuse irradiation reaching the surface past the blocked part of its sky, in kWh/m² per year.</param>
        /// <param name="ground">The annual ground-reflected irradiation reaching the surface past the blocked part of its ground, in kWh/m² per year.</param>
        /// <param name="irradiationUnshaded">The annual irradiation the surface would receive under an open sky, with no shading and nothing blocking its sky or ground, in kWh/m² per year.</param>
        /// <param name="energy">The annual solar energy incident on the whole surface with shading, in kWh per year.</param>
        /// <param name="skyVisibility">The unblocked share of the surface's isotropic sky view factor, from 0 (blocked) to 1 (open).</param>
        /// <param name="groundVisibility">The unblocked share of the surface's isotropic ground view factor, from 0 (blocked) to 1 (open).</param>
        public SurfaceSolarRadiationResult(
            string? reference,
            double area,
            double irradiation,
            double beam,
            double diffuse,
            double ground,
            double irradiationUnshaded,
            double energy,
            double skyVisibility,
            double groundVisibility)
            : base()
        {
            this.reference = reference;
            this.area = area;
            this.irradiation = irradiation;
            this.beam = beam;
            this.diffuse = diffuse;
            this.ground = ground;
            this.irradiationUnshaded = irradiationUnshaded;
            this.energy = energy;
            this.skyVisibility = skyVisibility;
            this.groundVisibility = groundVisibility;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceSolarRadiationResult"/> class by copying an existing instance.
        /// </summary>
        /// <param name="surfaceSolarRadiationResult">The source <see cref="SurfaceSolarRadiationResult"/> to copy from.</param>
        public SurfaceSolarRadiationResult(SurfaceSolarRadiationResult? surfaceSolarRadiationResult)
            : base(surfaceSolarRadiationResult)
        {
            if (surfaceSolarRadiationResult != null)
            {
                reference = surfaceSolarRadiationResult.reference;
                area = surfaceSolarRadiationResult.area;
                irradiation = surfaceSolarRadiationResult.irradiation;
                beam = surfaceSolarRadiationResult.beam;
                diffuse = surfaceSolarRadiationResult.diffuse;
                ground = surfaceSolarRadiationResult.ground;
                irradiationUnshaded = surfaceSolarRadiationResult.irradiationUnshaded;
                energy = surfaceSolarRadiationResult.energy;
                skyVisibility = surfaceSolarRadiationResult.skyVisibility;
                groundVisibility = surfaceSolarRadiationResult.groundVisibility;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceSolarRadiationResult"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The <see cref="JsonObject"/> containing the result data.</param>
        public SurfaceSolarRadiationResult(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets the area of the surface, in m².
        /// </summary>
        [JsonIgnore]
        public double Area
        {
            get
            {
                return area;
            }
        }

        /// <summary>
        /// Gets the annual beam (direct) irradiation reaching the unshaded part of the surface, in kWh/m² per year of the whole surface.
        /// </summary>
        [JsonIgnore]
        public double Beam
        {
            get
            {
                return beam;
            }
        }

        /// <summary>
        /// Gets the annual sky diffuse irradiation of the surface, reduced by the blocked part of its sky (<see cref="SkyVisibility"/>), in kWh/m² per year.
        /// </summary>
        [JsonIgnore]
        public double Diffuse
        {
            get
            {
                return diffuse;
            }
        }

        /// <summary>
        /// Gets the annual solar energy incident on the whole surface with shading, in kWh per year: <see cref="Irradiation"/> times <see cref="Area"/>.
        /// </summary>
        [JsonIgnore]
        public double Energy
        {
            get
            {
                return energy;
            }
        }

        /// <summary>
        /// Gets the annual ground-reflected irradiation of the surface, reduced by the blocked part of its ground (<see cref="GroundVisibility"/>), in kWh/m² per year.
        /// </summary>
        [JsonIgnore]
        public double Ground
        {
            get
            {
                return ground;
            }
        }

        /// <summary>
        /// Gets the unblocked share of the surface's isotropic ground view factor, from 0 (the building itself or its neighbours block the whole ground in front of it) to 1 (open).
        /// </summary>
        [JsonIgnore]
        public double GroundVisibility
        {
            get
            {
                return groundVisibility;
            }
        }

        /// <summary>
        /// Gets the annual irradiation of the surface with shading, in kWh/m² per year: the sum of <see cref="Beam"/>, <see cref="Diffuse"/> and <see cref="Ground"/>.
        /// </summary>
        [JsonIgnore]
        public double Irradiation
        {
            get
            {
                return irradiation;
            }
        }

        /// <summary>
        /// Gets the annual irradiation the surface would receive under an open sky, with no shading and nothing blocking its sky or ground, in kWh/m² per year. The difference to <see cref="Irradiation"/> is the loss to shading and blocked view.
        /// </summary>
        [JsonIgnore]
        public double IrradiationUnshaded
        {
            get
            {
                return irradiationUnshaded;
            }
        }

        /// <summary>
        /// Gets the <see cref="GuidReference"/> string of the building component the surface belongs to, the same string the shading model's element carries.
        /// </summary>
        [JsonIgnore]
        public string? Reference
        {
            get
            {
                return reference;
            }
        }

        /// <summary>
        /// Gets the unblocked share of the surface's isotropic sky view factor, from 0 (the building itself or its neighbours block the whole sky in front of it) to 1 (open).
        /// </summary>
        [JsonIgnore]
        public double SkyVisibility
        {
            get
            {
                return skyVisibility;
            }
        }
    }
}
