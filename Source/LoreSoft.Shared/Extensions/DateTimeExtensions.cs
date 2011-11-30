using System;
using System.Text;

namespace LoreSoft.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool Between(this DateTime value, DateTime start, DateTime end)
        {
            return (start <= value && value <= end);
        }
        public static bool Between(this DateTimeOffset value, DateTimeOffset start, DateTimeOffset end)
        {
            return (start <= value && value <= end);
        }


        public static string ToAgeString(this DateTime fromDate)
        {
            return ToAgeString(fromDate, DateTime.Now, 0);
        }

        public static string ToAgeString(this DateTime fromDate, int maxSpans)
        {
            return ToAgeString(fromDate, DateTime.Now, maxSpans);
        }

        public static string ToAgeString(this DateTime fromDate, DateTime toDate, int maxSpans)
        {
            return Age(fromDate, toDate).ToString(maxSpans);
        }

        public static DateSpan Age(this DateTime fromDate)
        {
            return Age(fromDate, DateTime.Now);
        }

        public static DateSpan Age(this DateTime fromDate, DateTime toDate)
        {
            DateSpan age = new DateSpan();

            int increment = 0;
            int day;

            // Day Calculation
            if (fromDate.Day > toDate.Day)
                increment = DateTime.DaysInMonth(fromDate.Year, fromDate.Month);

            if (increment != 0)
            {
                day = (toDate.Day + increment) - fromDate.Day;
                increment = 1;
            }
            else
                day = toDate.Day - fromDate.Day;

            age.Week = day / 7;
            age.Day = day % 7;

            //month calculation
            if ((fromDate.Month + increment) > toDate.Month)
            {
                age.Month = (toDate.Month + 12) - (fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                age.Month = (toDate.Month) - (fromDate.Month + increment);
                increment = 0;
            }

            // year calculation
            age.Year = toDate.Year - (fromDate.Year + increment);
            age.TimeSpan = toDate.Subtract(fromDate);

            return age;
        }

        /// <summary>
        /// Adjust the DateTime so the time is 1 millisecond before the next day.
        /// </summary>
        /// <param name="dateTime">The DateTime to adjust.</param>
        /// <returns>A DateTime that is 1 millisecond before the next day.</returns>
        public static DateTime ToEndOfDay(this DateTime dateTime)
        {
            return dateTime.Date                            // convet to just a date with out time
                .AddDays(1)                                 // add one day so its tomorow
                .Subtract(TimeSpan.FromMilliseconds(1));    // subtract 1 ms
        }

        private const UInt64 LocalMask = 0x8000000000000000;
        private const Int64 TicksCeiling = 0x4000000000000000;
        private const Int32 KindShift = 62;

        /// <summary>
        /// Serializes the current DateTime object to a 64-bit binary value that subsequently can be used to recreate the DateTime object.
        /// </summary>
        /// <param name="self">The DateTime to serialize.</param>
        /// <returns>A 64-bit signed integer that encodes the Kind and Ticks properties.</returns>
        /// <remarks>
        /// This method exists to add missing funtionality in Silverlight.
        /// </remarks>
        public static long ToBinary(this DateTime self)
        {
            // based on .net source code
            if (self.Kind != DateTimeKind.Local)
                return (self.Ticks | ((Int64)self.Kind << KindShift));

            // Local times need to be adjusted as you move from one time zone to another, 
            // just as they are when serializing in text. As such the format for local times
            // changes to store the ticks of the UTC time, but with flags that look like a 
            // local date. 

            // To match serialization in text we need to be able to handle cases where 
            // the UTC value would be out of range. Unused parts of the ticks range are
            // used for this, so that values just past max value are stored just past the
            // end of the maximum range, and values just below minimum value are stored
            // at the end of the ticks area, just below 2^62. 
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(self);
            Int64 ticks = self.Ticks;
            Int64 storedTicks = ticks - offset.Ticks;
            if (storedTicks < 0)
                storedTicks = TicksCeiling + storedTicks;

            return storedTicks | (unchecked((Int64)LocalMask));
        }
    }

    public struct DateSpan
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
        public int Day { get; set; }
        public TimeSpan TimeSpan { get; set; }

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int maxSpans)
        {
            int spanCount = 0;

            var sb = new StringBuilder();

            if (AppendSpan(sb, "year", Year, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();

            if (AppendSpan(sb, "month", Month, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();

            if (AppendSpan(sb, "week", Week, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();

            if (AppendSpan(sb, "day", Day, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();

            if (AppendSpan(sb, "hour", TimeSpan.Hours, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();

            if (AppendSpan(sb, "minute", TimeSpan.Minutes, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();

            if (AppendSpan(sb, "second", TimeSpan.Seconds, ref spanCount))
                if (maxSpans > 0 && spanCount >= maxSpans)
                    return sb.ToString();


            return sb.ToString();
        }

        private static bool AppendSpan(StringBuilder builder, string spanName, int spanValue, ref int total)
        {
            const string spacer = ", ";

            if (spanValue <= 0)
                return false;

            if (builder.Length > 0)
                builder.Append(spacer);

            builder.AppendFormat("{0} {1}{2}", spanValue, spanName, GetTense(spanValue));
            total++;
            return true;
        }

        private static string GetTense(int value)
        {
            return value > 1 ? "s" : string.Empty;
        }

    }
}
