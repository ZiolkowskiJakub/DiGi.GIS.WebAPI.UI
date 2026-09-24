using DiGi.Core.Classes;
using DiGi.Core.Enums;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial.Classes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;

namespace DiGi.GIS.WebAPI.UI.Controllers
{
    /// <summary>
    /// [TEMPORARY] Solar calculations for the 3D viewer Lighting panel, hosted locally until a
    /// DiGi.Solar backed endpoint is available on the central GIS Web API. The route contract
    /// (solar/sundirection) is final - when the central endpoint exists this controller becomes a
    /// proxy like the other controllers in this project, and the consuming frontend
    /// (gltf-viewer.js) stays unchanged.
    /// </summary>
    [Route("[controller]")]
    public class SolarController : Controller
    {
        /// <summary>
        /// Calculates the sun position for a world location and a local date and time.
        /// </summary>
        /// <param name="x">The X coordinate in the EPSG:2180 coordinate system [m].</param>
        /// <param name="y">The Y coordinate in the EPSG:2180 coordinate system [m].</param>
        /// <param name="date">The local calendar date in the yyyy-MM-dd format.</param>
        /// <param name="hour">The local time of day as a decimal hour in the 0-24 range.</param>
        /// <returns>JSON with the true solar angles: azimuth [deg] (0 = north, clockwise) and altitude [deg] above the horizon (negative at night).</returns>
        [HttpGet("sundirection")]
        public IActionResult GetSunDirection([FromQuery(Name = "x")] double x, [FromQuery(Name = "y")] double y, [FromQuery(Name = "date")] string? date, [FromQuery(Name = "hour")] double hour)
        {
            if (!double.IsFinite(x) || !double.IsFinite(y) || !double.IsFinite(hour) || hour < 0 || hour > 24)
            {
                return BadRequest();
            }

            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return BadRequest();
            }

            dateTime = dateTime.AddHours(hour);

            // GIS qualifier: this project's own Query class (DiGi.GIS.WebAPI.UI.Query) shadows
            // DiGi.GIS.Query in the enclosing-namespace lookup.
            Coordinates? coordinates = GIS.Query.Coordinates(new Point2D(x, y));
            if (coordinates is null)
            {
                return NoContent();
            }

            // EPSG:2180 scenes are Polish, so the local time zone is CET/CEST. The offset follows
            // the daylight saving state of the requested date, because this endpoint reports
            // wall-clock sun angles for a single date. A shading model, in contrast, holds one
            // fixed offset (UTC.Plus0100, no DST) for its whole solve - see UpdateBuildingInformation.
            // The identifier is the Windows form; .NET resolves it on any platform through the built-in IANA mapping.
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            UTC uTC = timeZoneInfo.GetUtcOffset(dateTime).TotalHours == 2 ? UTC.Plus0200 : UTC.Plus0100;

            Vector3D? vector3D = Solar.Query.SunDirection(coordinates, uTC, dateTime, true);
            if (vector3D is null)
            {
                return NoContent();
            }

            // SunDirection returns the direction sunlight travels, built from the solar angles as
            // x = cos(el) * cos(az + 90deg), y = -cos(el) * sin(az + 90deg), z = -sin(el);
            // inverted here back to the true solar angles the frontend contract expects.
            double altitude = Math.Asin(Math.Clamp(-vector3D.Z, -1, 1)) * (180.0 / Math.PI);
            double azimuth = (Math.Atan2(-vector3D.Y, vector3D.X) * (180.0 / Math.PI)) - 90.0;
            azimuth = ((azimuth % 360.0) + 360.0) % 360.0;

            return Ok(new { azimuth, altitude });
        }
    }
}