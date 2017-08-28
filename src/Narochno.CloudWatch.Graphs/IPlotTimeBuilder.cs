using OxyPlot;
using System.Threading.Tasks;

namespace Narochno.CloudWatch.Graphs
{
    public interface IPlotTimeBuilder
    {
        IPlotMetricBuilder AddMetric(string metricNamespace, string metricName);
        IPlotTimeBuilder WithTitle(string title, string subtitle = null);
        Task<PlotModel> Generate();
    }
}
