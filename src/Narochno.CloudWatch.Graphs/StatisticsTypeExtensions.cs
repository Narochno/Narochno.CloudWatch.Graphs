using System.Collections.Generic;

namespace Narochno.CloudWatch.Graphs
{
    internal static class StatisticsTypeExtensions
    {
        public static List<string> GetRequestStatistics(this StatisticType statisticType)
        {
            switch (statisticType)
            {
                case StatisticType.Average:
                case StatisticType.Maximum:
                case StatisticType.Minimum:
                case StatisticType.SampleCount:
                case StatisticType.Sum:
                    return new List<string> { statisticType.ToString() };
            }

            return null;
        }

        public static List<string> GetRequestExtendedStatistics(this StatisticType statisticType)
        {
            switch (statisticType)
            {
                case StatisticType.p10:
                case StatisticType.p50:
                case StatisticType.p90:
                case StatisticType.p95:
                case StatisticType.p99:
                    return new List<string> { statisticType.ToString() };
            }

            return null;
        }
    }
}
