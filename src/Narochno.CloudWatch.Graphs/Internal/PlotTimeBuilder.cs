using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Narochno.CloudWatch.Graphs.Internal
{
    public class PlotTimeBuilder : IPlotTimeBuilder
    {
        private readonly IList<PlotMetric> metrics = new List<PlotMetric>();
        private DateTime metricStartTime;
        private DateTime metricEndTime;
        private readonly IAmazonCloudWatch cloudWatch;
        private readonly ISeriesBuilder seriesBuilder;

        public PlotTimeBuilder(IAmazonCloudWatch cloudWatch, ISeriesBuilder seriesBuilder, DateTime metricStartTime, DateTime metricEndTime)
        {
            this.cloudWatch = cloudWatch;
            this.seriesBuilder = seriesBuilder;
            this.metricStartTime = metricStartTime;
            this.metricEndTime = metricEndTime;
        }

        public IPlotMetricBuilder AddMetric(string metricNamespace, string metricName)
        {
            var metric = new PlotMetric(metricNamespace, metricName, this);
            metrics.Add(metric);
            return metric;
        }

        public async Task<PlotModel> Generate()
        {
            var responses = metrics.ToDictionary(x => x, x => cloudWatch.GetMetricStatisticsAsync(new GetMetricStatisticsRequest
            {
                Namespace = x.Namespace,
                MetricName = x.Name,
                Period = (int)x.Period.TotalSeconds,
                StartTime = metricStartTime,
                Dimensions = x.Dimensions,
                EndTime = metricEndTime,
                Statistics = new List<string> { x.StatisticType.ToString() }
            }));

            await Task.WhenAll(responses.Select(x => x.Value));

            var model = new PlotModel
            {
                DefaultFont = "Arial",
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomLeft,
                LegendOrientation = LegendOrientation.Horizontal,
                DefaultColors = new List<OxyColor>
                {
                    OxyColor.FromRgb(31, 119, 180),
                    OxyColor.FromRgb(255, 127, 14),
                    OxyColor.FromRgb(44, 160, 44),
                    OxyColor.FromRgb(214, 39, 40),
                    OxyColor.FromRgb(44, 160, 44),
                    OxyColor.FromRgb(148, 103, 189),
                    OxyColor.FromRgb(140, 86, 75),
                    OxyColor.FromRgb(227, 119, 194),
                    OxyColor.FromRgb(127, 127, 127),
                    OxyColor.FromRgb(189, 189, 34),
                    OxyColor.FromRgb(23, 190, 207),
                    OxyColor.FromRgb(174, 199, 232)
                },
                PlotAreaBorderThickness = new OxyThickness(0d, 0d, 0d, 1d),
                PlotAreaBorderColor = OxyColor.FromRgb(204, 204, 204)
            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                TicklineColor = OxyColor.FromArgb(0, 0, 0, 0),
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(230, 230, 230),
                MinorGridlineStyle = LineStyle.Solid,
                MinorGridlineColor = OxyColor.FromRgb(244, 244, 244)
            };

            var aAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = DateTimeAxis.ToDouble(metricStartTime),
                Maximum = DateTimeAxis.ToDouble(metricEndTime),
                TicklineColor = OxyColor.FromRgb(238, 238, 238),
                StringFormat = GetTimeFormat()
            };

            model.Axes.Add(yAxis);
            model.Axes.Add(aAxis);

            foreach (var response in responses.Where(x => x.Value.Result.Datapoints.Any()))
            {
                var series = seriesBuilder.BuildSeries(response.Key, response.Value.Result.Datapoints);
                model.Series.Add(series);
            }

            return model;
        }

        public string GetTimeFormat()
        {
            var period = metricEndTime - metricStartTime;
            if (period > TimeSpan.FromDays(1))
            {
                return "MM/dd";
            }

            return "H:mm";
        }
    }
}
