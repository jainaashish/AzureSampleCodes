using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter.Geneva;
using OpenTelemetry.Metrics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace AshishTestFunction
{
    public class Function1
    {
        private readonly SliMetrics _sliMetrics;
        public Function1(SliMetrics sliMetrics)
        {
            _sliMetrics = sliMetrics;
        }

        [FunctionName("Function1")]
        public void Run([TimerTrigger("0 */1 * * * *", RunOnStartup =true)]TimerInfo myTimer, ILogger log)
        {
            var tags = new KeyValuePair<string, object?>[]
            {
                new KeyValuePair<string, object?>("CustomerResourceId","QualityAnalytics"),
                new KeyValuePair<string, object?>("LocationId","public/west us 3"),
                new KeyValuePair<string, object?>("IsSuccess",true)
            };

            _sliMetrics.Record(4000, tags);
            _sliMetrics.Record(4020, tags);
            _sliMetrics.Record(5000, tags);
        }
    }
}
