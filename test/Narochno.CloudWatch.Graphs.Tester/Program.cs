using Amazon;
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

            var plotModel = await plotBuilder.WithTime(DateTime.UtcNow.AddDays(-4), DateTime.UtcNow)
                .AddMetric("AWS/Logs", "IncomingBytes")
                    .PlotGraph(GraphType.Line, StatisticType.Average, TimeSpan.FromMinutes(5))
                .Generate();

            var svgExporter = new SvgExporter
            {
                Width = 750,
                Height = 300
            };

            File.Delete("test.svg");

            using (var output = File.OpenWrite("test.svg"))
            {
                svgExporter.Export(plotModel, output);
            }
        }
    }
}