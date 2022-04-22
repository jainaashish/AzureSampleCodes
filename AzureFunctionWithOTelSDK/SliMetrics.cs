using System.Collections.Generic;
using System.Diagnostics.Metrics;


namespace AshishTestFunction
{
    public class SliMetrics
    {
        // Define a Histogram instrument using the meter defined above
        private readonly Histogram<long> _responseLatencyHistogram;

        public SliMetrics(Histogram<long> responseLatencyHistogram)
        {
            _responseLatencyHistogram = responseLatencyHistogram;
        }

        public void Record(long duration, KeyValuePair<string, object?>[] tags)
        {
            _responseLatencyHistogram.Record(duration, tags);
        }
    }
}
