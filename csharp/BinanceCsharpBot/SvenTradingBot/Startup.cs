using BinanceImporter.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SvenTrading.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvenTradingBot
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Func<SvenTradingContext> factoryDbContext = () => CreateSvenTradingContext();       

            services.AddTransient<Func<SvenTradingContext>>(provider => factoryDbContext);

            services.AddTransient<ITradeSignalRepository, TradeSignalRepository>();

            services.AddHostedService<Worker>();
        }

        private SvenTradingContext CreateSvenTradingContext()
        {
            string connectionString = Configuration.GetConnectionString("MyConnectionString");
            return new SvenTradingContext(connectionString);
        }

    }
}
