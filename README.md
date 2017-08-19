# Narochno.CloudWatch.Graphs [![Build status](https://ci.appveyor.com/api/projects/status/7p7owa4ksp8w2jk2/branch/master?svg=true)](https://ci.appveyor.com/project/Narochno/narochno-cloudwatch-graphs/branch/master) [![NuGet](https://img.shields.io/nuget/v/Narochno.CloudWatch.Graphs.svg)](https://www.nuget.org/packages/Narochno.CloudWatch.Graphs/)
A fluent library for exporting graph images from CloudWatch metrics.

## Supported Frameworks
* netstandard1.3 and above
* net45 and above

## Example Usage

![Example](example.png)

```csharp
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

using (var output = File.Open("example.svg", FileMode.Create))
{
    svgExporter.Export(plotModel, output);
}
```
