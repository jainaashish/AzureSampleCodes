using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter.Geneva;
using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;

[assembly: FunctionsStartup(typeof(AshishTestFunction.Startup))]
namespace AshishTestFunction;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var MyServiceMeter = new Meter("Microsoft.Quality.SliSloAnalytics", "1.0");
        var responseLatencyHistogram = MyServiceMeter.CreateHistogram<long>("testMetric");

        // Create a double[] to provide custom bucket bounds for the histogram
        // By default, OpenTelemetry SDK uses [ 0, 5, 10, 25, 50, 75, 100, 250, 500, 1000 ] as the bucket values as per the OpenTelemetry specification
        // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/metrics/sdk.md#explicit-bucket-histogram-aggregation
        var minValue = 10;
        var bucketSize = 10;
        var bucketCount = 2000;
        var firstBucketValue = minValue;
        var customBucketBounds = new double[bucketCount];
        for (int i = 0; i < bucketCount; i++)
        {
            customBucketBounds[i] = firstBucketValue;
            firstBucketValue += bucketSize;
        }

        var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter(MyServiceMeter.Name)
            .AddView(responseLatencyHistogram.Name, 
                new ExplicitBucketHistogramConfiguration() 
                { 
                    Boundaries = customBucketBounds 
                })
            .AddGenevaMetricExporter(options =>
            {
                options.ConnectionString = $"Account=testMonitoring;Namespace=testNamespace";
            })
            .Build();

        builder.Services.AddSingleton(meterProvider);
        builder.Services.AddSingleton(new SliMetrics(responseLatencyHistogram));   
    }
}

