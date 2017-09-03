using Amazon.CloudWatch;
using Humanizer;
using System;

namespace Narochno.CloudWatch.Graphs
{
    internal static class StandardUnitExtensions
    {
        internal const string DoubleFormatting = "#.##";

        internal static Func<double, string> GetLabelFormatter(this StandardUnit unit)
        {
            if (unit == StandardUnit.Bits)
            {
                return (value) => ((long)value).Bits().Humanize(DoubleFormatting);
            }
            if (unit == StandardUnit.BitsSecond)
            {
                return (value) => ((long)value).Bits().Per(TimeSpan.FromSeconds(1)).Humanize(DoubleFormatting);
            }
            if (unit == StandardUnit.Kilobytes)
            {
                return (value) => value.Kilobytes().Humanize(DoubleFormatting);
            }
            if (unit == StandardUnit.KilobytesSecond)
            {
                return (value) => value.Kilobytes().Per(TimeSpan.FromSeconds(1)).Humanize(DoubleFormatting);
            }
            if (unit == StandardUnit.Bytes)
            {
                return (value) => value.Bytes().Humanize(DoubleFormatting);
            }
            if (unit == StandardUnit.BytesSecond)
            {
                return (value) => value.Bytes().Per(TimeSpan.FromSeconds(1)).Humanize(DoubleFormatting);
            }
            if (unit == StandardUnit.Megabytes)
            {
                return (value) => value.Megabytes().Humanize(DoubleFormatting);
            }
            if (unit == StandardUnit.MegabytesSecond)
            {
                return (value) => value.Megabytes().Per(TimeSpan.FromSeconds(1)).Humanize(DoubleFormatting);
            }
            if (unit == StandardUnit.Gigabytes)
            {
                return (value) => value.Gigabytes().Humanize(DoubleFormatting);
            }
            if (unit == StandardUnit.GigabytesSecond)
            {
                return (value) => value.Gigabytes().Per(TimeSpan.FromSeconds(1)).Humanize(DoubleFormatting);
            }
            if (unit == StandardUnit.Terabytes)
            {
                return (value) => value.Terabytes().Humanize(DoubleFormatting);
            }
            if (unit == StandardUnit.TerabytesSecond)
            {
                return (value) => value.Terabytes().Per(TimeSpan.FromSeconds(1)).Humanize(DoubleFormatting);
            }
            if (unit == StandardUnit.Milliseconds)
            {
                return (value) => TimeSpan.FromMilliseconds(value).Humanize();
            }
            if (unit == StandardUnit.Seconds)
            {
                return (value) => TimeSpan.FromSeconds(value).Humanize();
            }
            if (unit == StandardUnit.Microseconds)
            {
                return (value) => value.ToString(DoubleFormatting) + " μs";
            }
            if (unit == StandardUnit.Percent)
            {
                return (value) => value.ToString(DoubleFormatting) + "%";
            }

            return (value) => value.ToString("N0");
        }
    }
}
