using System;

namespace DiGi.GIS.WebAPI.UI
{
    public static partial class Query
    {
        /// <summary>
        /// Maps the time stamp of an EPW record to the instant the solar radiation calculation samples the sun at: the middle of the hour the record covers, in <see cref="Constants.Default.SolarReferenceYear"/>.
        /// <para>An EPW record is stamped with the end of its hour (<c>EPW.Convert.ToSystem_DateTime</c>: hour 1 is 01:00) and its radiation is the integral over the hour before, so the sun is sampled 30 minutes earlier. The time is local standard time, the fixed offset of the shading model, so no daylight saving shift applies.</para>
        /// <para>Typical meteorological years mix calendar years month by month, and hour 24 of 31 December rolls into 1 January of the next year; moving the mid-hour instant into one fixed non-leap year keeps the series monotonic and one hour per record. 31 December 24:00 therefore maps to 31 December 23:30 of the reference year.</para>
        /// </summary>
        /// <param name="dateTime">The hour-ending time stamp of the EPW record.</param>
        /// <returns>The mid-hour instant in the reference year, or <see langword="null"/> for an hour on 29 February, which a non-leap year does not have.</returns>
        public static DateTime? SolarReferenceDateTime(this DateTime dateTime)
        {
            DateTime dateTime_Middle = dateTime.AddMinutes(-30);
            if (dateTime_Middle.Month == 2 && dateTime_Middle.Day == 29)
            {
                return null;
            }

            return new DateTime(Constants.Default.SolarReferenceYear, dateTime_Middle.Month, dateTime_Middle.Day, dateTime_Middle.Hour, dateTime_Middle.Minute, dateTime_Middle.Second, dateTime_Middle.Kind);
        }
    }
}
