namespace ProcessingService
{
    using Serilog;

    using Topshelf;
    using Topshelf.Logging;

    public class Program
    {
        private static int Main(string[] args)
        {
            var logger =new LoggerConfiguration()
                .WriteTo.ColoredConsole().CreateLogger();
            Log.Logger = logger;
            
            SerilogLogWriterFactory.Use(logger);
            
            return (int)HostFactory.Run(x => x.Service<ActivityService>());
        }
    }
}