using BinanceImporter.Config;
using BinanceImporter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BinanceImporter.Infrastructure
{
    public class TestDBContext : DbContext
    {

        //static LoggerFactory object
        public static ILoggerFactory loggerFactory;
        private readonly ImporterConfig importerConfig;

        public TestDBContext(ImporterConfig importerConfig)
        {
            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole(options => options.IncludeScopes = true);
            });
            this.importerConfig = importerConfig;
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>()
                .HavePrecision(18, 8);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(loggerFactory)  //tie-up DbContext with LoggerFactory object
                .EnableSensitiveDataLogging()
                .UseSqlServer(importerConfig.DbConnectionString);
        }

        public DbSet<TradeData> TradeData { get; set; }
    }

}