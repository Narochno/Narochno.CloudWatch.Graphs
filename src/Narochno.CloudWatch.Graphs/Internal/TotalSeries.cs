using OxyPlot.Series;
using System;
using OxyPlot;
using OxyPlot.Axes;
using System.Collections.Generic;
using Amazon.CloudWatch.Model;
using System.Linq;
using Humanizer;
using System.Globalization;

namespace Narochno.CloudWatch.Graphs.Internal
{
    internal class TotalSeries : Series
    {
        private readonly PlotMetric metric;
        private readonly IList<Datapoint> dataPoints;

        public TotalSeries(PlotMetric metric, IList<Datapoint> dataPoints)
        {
            this.metric = metric;
            this.dataPoints = dataPoints;
        }

        public double TotalFontSize => PlotModel.DefaultFontSize * 4;

        public double GetTotal()
        {
            if (metric.StatisticType == StatisticType.Sum)
            {
                return dataPoints.Sum(x => x.Sum);
            }

            return dataPoints.Sum(x => x.StatisticTypeValue(metric.StatisticType)) / dataPoints.Count;
        }

        public string GetTotalText()
        {
            if (dataPoints.Count == 0)
            {
                return "0";
            }

            return dataPoints.First().Unit.GetLabelFormatter()(GetTotal());
        }

        public override void Render(IRenderContext rc)
        {
            var numberOfSeries = PlotModel.Series.OfType<TotalSeries>().Count();
            var slotWidth = PlotModel.Width / numberOfSeries;

            var slotNumber = PlotModel.Series.IndexOf(this);
            var startOffset = slotNumber * slotWidth;
            var endOffset = (slotNumber + 1) * slotWidth;
            var middleHorizontalOffset = startOffset + (slotWidth / 2);
            var middleVerticalOffset = PlotModel.Height / 2;
            var slotColor = PlotModel.DefaultColors[slotNumber];

            var label = GetTotalText();

            var totalTextSize = rc.MeasureText(label, PlotModel.DefaultFont, TotalFontSize);
            var totalTextPosition = new ScreenPoint(middleHorizontalOffset - (totalTextSize.Width / 2), middleVerticalOffset - (totalTextSize.Height / 2));

            var keyShapeSize = new OxySize(PlotModel.DefaultFontSize, PlotModel.DefaultFontSize);
            var keyShapePadding = PlotModel.DefaultFontSize;
            var keyTextSize = rc.MeasureText(metric.GetTitle(), PlotModel.DefaultFont, PlotModel.DefaultFontSize);

            var keyWidth = keyShapeSize.Width + keyTextSize.Width;
            var keyHorizontalOffset = middleHorizontalOffset - (keyWidth + keyShapePadding) / 2;

            var keyShapePosition = new ScreenPoint(keyHorizontalOffset, middleVerticalOffset + totalTextSize.Height);
            var keyTextPosition = new ScreenPoint(keyHorizontalOffset + keyShapeSize.Width + keyShapePadding, middleVerticalOffset + totalTextSize.Height);

            rc.DrawText(totalTextPosition, label, PlotModel.TextColor, PlotModel.DefaultFont, TotalFontSize);
            rc.DrawRectangle(new OxyRect(keyShapePosition, keyShapeSize), slotColor, new OxyColor());
            rc.DrawText(keyTextPosition, metric.GetTitle(), PlotModel.TextColor, PlotModel.DefaultFont, PlotModel.DefaultFontSize);
        }

        public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
        {
        }

        protected override bool AreAxesRequired() => false;

        protected override void EnsureAxes()
        {
        }

        protected override bool IsUsing(Axis axis) => false;

        protected override void SetDefaultValues()
        {
        }

        protected override void UpdateAxisMaxMin()
        {
        }

        protected override void UpdateData()
        {
        }

        protected override void UpdateMaxMin()
        {
        }

        protected override void UpdateValidData()
        {
        }
    }
}
