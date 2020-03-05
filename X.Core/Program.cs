using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace X.Core
{
    class Program
    {
        static void Main()
        {
            new HostBuilder().ConfigureServices(s => s.AddSingleton<IHostedService, PerformanceMetricsCollector>())
                             .Build()
                             .Run();
        }
    }


    public sealed class PerformanceMetricsCollector : IHostedService
    {
        private IDisposable _scheduler;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _scheduler = new Timer(Callback, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }
        static void Callback(object state)
        {
            Console.WriteLine($"[{DateTimeOffset.Now}]{PerformanceMetrics.Create()}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _scheduler?.Dispose();
            return Task.CompletedTask;
        }
    }


    public class PerformanceMetrics {
        private static readonly Random _random = new Random();

        public int Processor { get; set; }

        public long Memory { get; set; }

        public long Network { get; set; }

        public override string ToString() => $"CPU: {Processor * 100}%; Memory: {Memory / (1024 * 1024)}M; Network: {Network / (1024 * 1024)}M/s";

        public static PerformanceMetrics Create() => new PerformanceMetrics
        {
            Processor = _random.Next(1,8),
            Memory = _random.Next(10,100)*1024*1024,
            Network = _random.Next(10,100)*1024*1024

        };
    }
}
