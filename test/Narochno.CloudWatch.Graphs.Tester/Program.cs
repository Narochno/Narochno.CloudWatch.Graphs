﻿using Amazon;
using Amazon.CloudWatch;
using Microsoft.Extensions.DependencyInjection;
using OxyPlot;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Narochno.CloudWatch.Graphs.Tester
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DoWork().Wait();
            }
            catch (AggregateException e)
            {
                throw e.Flatten();
            }
        }

        public static async Task DoWork()
        {
            var provider = new ServiceCollection()
                .AddSingleton<IAmazonCloudWatch>(new AmazonCloudWatchClient(RegionEndpoint.EUWest1))
                .AddCloudWatchGraphs()
                .BuildServiceProvider();

            var plotBuilder = provider.GetService<IPlotBuilder>();

            var builder = plotBuilder.WithTime(DateTime.UtcNow.AddDays(-4), DateTime.UtcNow)
                .AddMetric("AWS/Logs", "IncomingBytes")
                    .PlotGraph(GraphType.Line, StatisticType.Average, TimeSpan.FromMinutes(5))
                .AddMetric("AWS/Logs", "IncomingLogEvents")
                    .PlotGraph(GraphType.Line, StatisticType.Average, TimeSpan.FromMinutes(5));

            var model = await builder.Generate();


            var svgExporter = new SvgExporter();

            svgExporter.Width = 1000;
            svgExporter.Height = 300;

            File.Delete("test.svg");

            using (var output = File.OpenWrite("test.svg"))
            {
                svgExporter.Export(model, output);
            }
        }
    }
}