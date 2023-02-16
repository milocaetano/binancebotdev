using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SvenTrading.Domain.Domain.Entities;
using SvenTrading.Domain.Entities;

namespace BinanceImporter.Infrastructure
{
    public class SvenTradingContext : DbContext
    {

        //static LoggerFactory object
        public static ILoggerFactory loggerFactory;  
        private readonly string stringconnection;

        public SvenTradingContext(string stringconnection)
        {
            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole(options => options.IncludeScopes = true);
            });
       
            this.stringconnection = stringconnection;
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
                .UseSqlServer(stringconnection);
        }

        public DbSet<TradeData> TradeData { get; set; }
        public DbSet<TradeSignal> TradeSignal { get; set; }
    }

}