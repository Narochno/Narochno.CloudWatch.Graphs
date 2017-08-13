using Amazon.CloudWatch.Model;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Narochno.CloudWatch.Graphs.Internal
{
    public interface ISeriesBuilder
    {
        Series BuildSeries(PlotMetric metric, IList<Datapoint> dataPoints);
    }

    public class SeriesBuilder : ISeriesBuilder
    {
        public Series BuildSeries(PlotMetric metric, IList<Datapoint> dataPoints)
        {
            switch (metric.GraphType)
            {
                case GraphType.Area:
                    return BuildXYSeries(metric, dataPoints, new AreaSeries { StrokeThickness = 1.5d });
                case GraphType.Stair:
                    return BuildXYSeries(metric, dataPoints, new StairStepSeries { StrokeThickness = 1.5d });
                case GraphType.Line:
                    return BuildXYSeries(metric, dataPoints, new LineSeries { StrokeThickness = 1.5d });
                default:
                    throw new NotImplementedException($"Graph type {metric.GraphType} not supported");
            }
        }

        public XYAxisSeries BuildXYSeries(PlotMetric metric, IList<Datapoint> dataPoints, XYAxisSeries series)
        {
            series.Title = metric.GetTitle();

            var points = metric.GetIncrements(dataPoints.Min(x => x.Timestamp), dataPoints.Max(x => x.Timestamp))
                               .ToDictionary(x => x, y => double.NaN);

            foreach (var dataPoint in dataPoints)
            {
                if (!points.ContainsKey(dataPoint.Timestamp))
                {
                    throw new InvalidOperationException($"Generated time slice array doesn't contain {dataPoint.Timestamp}");
                }

                switch (metric.StatisticType)
                {
                    case StatisticType.Average:
                        points[dataPoint.Timestamp] = dataPoint.Average;
                        break;
                    case StatisticType.Sum:
                        points[dataPoint.Timestamp] = dataPoint.Sum;
                        break;
                    case StatisticType.SampleCount:
                        points[dataPoint.Timestamp] = dataPoint.SampleCount;
                        break;
                    case StatisticType.Maximum:
                        points[dataPoint.Timestamp] = dataPoint.Maximum;
                        break;
                    case StatisticType.Minimum:
                        points[dataPoint.Timestamp] = dataPoint.Minimum;
                        break;
                    default:
                        throw new InvalidOperationException($"Statistic type {metric.StatisticType} not supported");
                }
            }

            series.ItemsSource = points.Select(x => DateTimeAxis.CreateDataPoint(x.Key, x.Value));
            return series;
        }
    }
}
