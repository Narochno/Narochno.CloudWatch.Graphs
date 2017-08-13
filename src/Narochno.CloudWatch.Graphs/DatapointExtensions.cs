using Amazon.CloudWatch.Model;
using System;

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
                default:
                    throw new InvalidOperationException($"Statistic type {statisticType} not supported");
            }
        }
    }
}
