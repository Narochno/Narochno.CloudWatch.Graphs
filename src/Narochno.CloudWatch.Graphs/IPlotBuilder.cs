using System;

namespace Narochno.CloudWatch.Graphs
{
    public interface IPlotBuilder
    {
        IPlotTimeBuilder WithTime(DateTime metricsStartTime, DateTime metricsEndTime);
    }
}
