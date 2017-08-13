﻿using System;

namespace Narochno.CloudWatch.Graphs
{
    public interface IPlotMetricBuilder
    {
        IPlotTimeBuilder PlotGraph(GraphType graphType, StatisticType statisticType, TimeSpan period);
        IPlotMetricBuilder WithDimension(string dimensionName, string dimensionValue);
    }
}
