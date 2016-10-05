namespace TrackingService
{
    using Serilog;

    using Topshelf;

    internal class Program
    {
        private static int Main(string[] args)
        {
            ConfigureLogger();
            
            return (int)HostFactory.Run(x => x.Service<TrackingService>());
        }

        private static void ConfigureLogger()
        {
            var logger = new LoggerConfiguration().WriteTo.ColoredConsole().CreateLogger();
            Log.Logger = logger;
        }
    }
}