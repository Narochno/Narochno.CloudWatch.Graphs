using Microsoft.Extensions.DependencyInjection;
using Narochno.CloudWatch.Graphs.Internal;

namespace Narochno.CloudWatch.Graphs
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCloudWatchGraphs(this IServiceCollection services)
        {
            return services.AddSingleton<ISeriesBuilder, SeriesBuilder>()
                           .AddSingleton<IPlotBuilder, PlotBuilder>();
        }
    }
}
