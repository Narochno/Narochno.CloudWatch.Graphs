using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Humanizer;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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
        private string title;
        private string subtitle;

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
                Statistics = x.StatisticType.GetRequestStatistics(),
                ExtendedStatistics = x.StatisticType.GetRequestExtendedStatistics()
            }));

            await Task.WhenAll(responses.Select(x => x.Value));

            var model = new PlotModel
            {
                Title = title,
                Subtitle = subtitle,
                Padding = new OxyThickness(20d),
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomLeft,
                LegendOrientation = LegendOrientation.Horizontal,
                DefaultColors = GraphConstants.Colors,
                PlotAreaBorderThickness = new OxyThickness(0d, 0d, 0d, 1d),
                PlotAreaBorderColor = OxyColor.FromRgb(204, 204, 204)
            };

            var dataRanges = new List<Tuple<StandardUnit, double>>();

            foreach (var response in responses.Where(x => x.Value.Result.Datapoints.Any()))
            {
                IEnumerable<Datapoint> orderedData = response.Value.Result.Datapoints.OrderByDescending(i => i.StatisticTypeValue(response.Key.StatisticType));

                Datapoint highestDataPoint = orderedData.First();
                Datapoint lowestDataPoint = orderedData.Last();

                dataRanges.Add(Tuple.Create(highestDataPoint.Unit, highestDataPoint.StatisticTypeValue(response.Key.StatisticType)));
                dataRanges.Add(Tuple.Create(lowestDataPoint.Unit, lowestDataPoint.StatisticTypeValue(response.Key.StatisticType)));

                Series series = seriesBuilder.BuildSeries(response.Key, response.Value.Result.Datapoints);
                model.Series.Add(series);
            }

            var aAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = DateTimeAxis.ToDouble(metricStartTime),
                Maximum = DateTimeAxis.ToDouble(metricEndTime),
                TicklineColor = OxyColor.FromRgb(238, 238, 238),
                StringFormat = GetTimeFormat()
            };

            model.Axes.Add(InferYAxis(dataRanges));
            model.Axes.Add(aAxis);

            return model;
        }

        public Axis InferYAxis(IList<Tuple<StandardUnit, double>> dataRanges)
        {
            double highest = dataRanges.Any() ? dataRanges.Max(x => x.Item2) : 0d;
            double lowest = dataRanges.Any() ? dataRanges.Min(x => x.Item2) : 0d;

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = lowest,
                IntervalLength = 30,
                Maximum = highest,
                TicklineColor = OxyColor.FromArgb(0, 0, 0, 0),
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(230, 230, 230),
                MinorGridlineStyle = LineStyle.Solid,
                MinorGridlineColor = OxyColor.FromRgb(244, 244, 244)
            };

            if (!dataRanges.Any())
            {
                return yAxis;
            }

            StandardUnit unit = dataRanges.First().Item1;

            // Only apply formatting for specific unit
            // if all data points are using the same
            if (dataRanges.All(x => x.Item1 == unit))
            {
                yAxis.LabelFormatter = unit.GetLabelFormatter();
            }

            return yAxis;
        }

        public string GetTimeFormat()
        {
            TimeSpan period = metricEndTime - metricStartTime;
            if (period > TimeSpan.FromDays(180))
            {
                return "MMM/yyyy";
            }

            if (period > TimeSpan.FromDays(1))
            {
                return "dd/MMM";
            }

            if (period > TimeSpan.FromMinutes(5))
            {
                return "H:mm";
            }

            return "H:mm:ss";
        }

        public IPlotTimeBuilder WithTitle(string title, string subtitle)
        {
            this.title = title;
            this.subtitle = subtitle;
            return this;
        }
    }
}
