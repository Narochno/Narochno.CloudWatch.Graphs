using Amazon.CloudWatch.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Narochno.CloudWatch.Graphs.Internal
{
    public class PlotMetric : IPlotMetricBuilder
    {
        private readonly PlotBuilder plotBuilder;

        public PlotMetric(string metricNamespace, string metricName, PlotBuilder plotBuilder)
        {
            Name = metricName;
            Namespace = metricNamespace;
            this.plotBuilder = plotBuilder;
        }

        public string Name { get; private set; }
        public List<Dimension> Dimensions { get; private set; } = new List<Dimension>();
        public TimeSpan Period { get; private set; }
        public StatisticType StatisticType { get; private set; }
        public GraphType GraphType { get; private set; }
        public string Namespace { get; private set; }

        public IPlotMetricBuilder WithDimension(string dimensionName, string dimensionValue)
        {
            Dimensions.Add(new Dimension { Name = dimensionName, Value = dimensionValue });
            return this;
        }

        public string GetTitle()
        {
            if (Dimensions.Any())
            {
                return $"{string.Join(",", Dimensions.Select(x => x.Value))} {Name}";
            }

            return Name;
        }

        public IEnumerable<DateTime> GetIncrements(DateTime firstIncrement, DateTime lastIncrement)
        {
            DateTime current = firstIncrement;
            do
            {
                yield return current;
                current += Period;
            }
            while (current < lastIncrement);

            yield return current;
        }

        public IPlotTimeBuilder PlotGraph(GraphType graphType, StatisticType statisticType, TimeSpan period)
        {
            StatisticType = statisticType;
            GraphType = graphType;
            Period = period;
            return plotBuilder;
        }
    }
}
