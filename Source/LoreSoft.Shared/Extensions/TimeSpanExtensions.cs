using System;
using System.Collections.Generic;

namespace LoreSoft.Shared.Extensions
{
    public static class TimeSpanExtensions
    {
        public static double Year(this TimeSpan span)
        {
            return span.TotalDays / 365d;
        }

        public static double Month(this TimeSpan span)
        {
            return span.TotalDays / 30.5d;
        }

        public static double Week(this TimeSpan span)
        {
            return span.TotalDays / 7d;
        }

        public static double Microsecond(this TimeSpan span)
        {
            return span.Ticks / 10d;
        }

        public static double Nanosecond(this TimeSpan span)
        {
            return span.Ticks / 100d;
        }

        public static string ToWords(this TimeSpan span)
        {
            return ToWords(span, false);
        }

        public static string ToWords(this TimeSpan span, bool shortForm)
        {
            var timeStrings = new List<string>();

            var timeParts = new List<double>(new[] { (double)span.Days, span.Hours, span.Minutes, span.Seconds });
            var timeUnits = new List<string>();
            timeUnits.AddRange(shortForm
                                   ? new[] { "d", "h", "m", "s" }
                                   : new[] { "day", "hour", "minute", "second" });

            if (span.TotalSeconds < 10)
            {
                timeParts[3] = Math.Round(span.TotalSeconds, 2);
            }

            for (int i = 0; i < timeParts.Count; i++)
            {
                if (timeParts[i] > 0)
                {
                    timeStrings.Add(String.Format(shortForm ? "{0}{1}" : "{0} {1}", timeParts[i], shortForm ? timeUnits[i] : Pluralize(timeParts[i], timeUnits[i])));
                }
            }

            return timeStrings.Count != 0 ? String.Join(" ", timeStrings.ToArray()) : shortForm ? "0s" : "0 seconds";
        }

        private static string Pluralize(double n, string unit)
        {
            if (String.IsNullOrEmpty(unit))
                return String.Empty;

            n = Math.Abs(n); // -1 should be singular, too

            return unit + (n == 1 ? string.Empty : "s");
        }
    }
}
