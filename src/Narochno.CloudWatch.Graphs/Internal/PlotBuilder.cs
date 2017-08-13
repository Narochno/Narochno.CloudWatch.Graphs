using Amazon.CloudWatch;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Narochno.CloudWatch.Graphs.Internal
{
    public class PlotBuilder : IPlotBuilder
    {
        private readonly IServiceProvider serviceProvider;

        public PlotBuilder(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IPlotTimeBuilder WithTime(DateTime metricsStartTime, DateTime metricsEndTime)
        {
            return new PlotTimeBuilder(serviceProvider.GetRequiredService<IAmazonCloudWatch>(),
                serviceProvider.GetRequiredService<ISeriesBuilder>(), metricsStartTime, metricsEndTime);
        }
    }
}
