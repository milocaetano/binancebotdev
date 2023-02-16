using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.RollingFile;
using SvenTradingBot.LogConfiguration;

namespace SvenTradingBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console();

            var serilogConfig = configuration.GetSection("Serilog");
            foreach (var writeToSection in serilogConfig.GetSection("WriteTo").GetChildren())
            {
                var minimumLevel = writeToSection.GetSection("MinimumLevel").Value;
                var pathFormat = writeToSection.GetSection("Args:pathFormat").Value;
                var sink = new RollingFileSink(pathFormat, new AWSTextFormatter(), null, null, null, false, true);

                switch (minimumLevel)
                {
                    case "Information":
                        loggerConfiguration.WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
                            .WriteTo.Sink(sink));
                        break;
                    case "Warning":
                        loggerConfiguration.WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
                            .WriteTo.Sink(sink));
                        break;
                    case "Trace":
                        loggerConfiguration.WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Verbose)
                            .WriteTo.Sink(sink));
                        break;
                    case "Error":
                        loggerConfiguration.WriteTo.Logger(lc => lc
                            .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                            .WriteTo.Sink(sink));
                        break;
                }
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices((hostContext, services) =>
                new Startup(hostContext.Configuration).ConfigureServices(services))
                .UseSerilog()
                .Build();

            host.Run();
        }
    }


}