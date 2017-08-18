using Amazon.CloudWatch.Model;
using System;
using System.Linq;

namespace Narochno.CloudWatch.Graphs
{
    public static class DatapointExtensions
    {
        public static double StatisticTypeValue(this Datapoint dataPoint, StatisticType statisticType)
        {
            switch (statisticType)
            {
                case StatisticType.Average:
                    return dataPoint.Average;
                case StatisticType.Sum:
                    return dataPoint.Sum;
                case StatisticType.SampleCount:
                    return dataPoint.SampleCount;
                case StatisticType.Maximum:
                    return dataPoint.Maximum;
                case StatisticType.Minimum:
                    return dataPoint.Minimum;
                case StatisticType.p10:
                case StatisticType.p50:
                case StatisticType.p90:
                case StatisticType.p95:
                case StatisticType.p99:
                    return dataPoint.ExtendedStatistics.Single().Value;
                default:
                    throw new InvalidOperationException($"Statistic type {statisticType} not supported");
            }
        }
    }
}
