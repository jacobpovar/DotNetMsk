namespace TrackingService
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    
    public class RoutingSlipMetrics
    {
        private readonly ConcurrentBag<TimeSpan> durations;

        private long completedCount;

        private readonly string description;

        public RoutingSlipMetrics(string description)
        {
            this.description = description;
            this.completedCount = 0;
            this.durations = new ConcurrentBag<TimeSpan>();
        }

        public void AddComplete(TimeSpan duration)
        {
            var count = Interlocked.Increment(ref this.completedCount);
            this.durations.Add(duration);

            if (count % 1 == 0) this.Snapshot();
        }

        private void Snapshot()
        {
            var snapshot = this.durations.ToArray();
            var averageDuration = snapshot.Average(x => x.TotalMilliseconds);

            Console.WriteLine("{0} {2} Completed, {1:F0}ms (average)", snapshot.Length, averageDuration, this.description);
        }
    }
}