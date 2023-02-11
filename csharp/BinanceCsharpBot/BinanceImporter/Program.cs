
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;

namespace BinanceImporter
{
    public static class StringExtensions
    {
        public static string RemoveNonNumeric(this string input)
        {
            string output = string.Empty;

            foreach (char c in input)
            {
                if (Char.IsDigit(c) || c == '.')
                {
                    output += c;
                }
            }

            return output;
        }
    }

    class Program
    {

        public static string LimpaEsseCariaio( string input)
        {
            string output = string.Empty;

            foreach (char c in input)
            {
                if (Char.IsDigit(c) || c == '.')
                {
                    output += c;
                }
            }

            return output;
        }


        static void Main(string[] args)
        {
            var connectionString = "Data Source=(local);Initial Catalog=SvenTrading;Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=true";
            var optionsBuilder = new DbContextOptionsBuilder<TestDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using (var context = new TestDBContext(optionsBuilder.Options))
            {
                List<Transaction> transactions = new List<Transaction>();

                using (var reader = new StreamReader(@"C:\temp\binance.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    transactions = csv.GetRecords<Transaction>().ToList();
                }

                List<TradeData> listaDeTradeDatas = new List<TradeData>();
           
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Transaction, TradeData>()
                         .ForMember(dest => dest.DateUTC, opt => opt.MapFrom(src => src.Date))
                        .ForMember(dest => dest.Executed, opt => opt.MapFrom(src => Decimal.Parse(src.Executed.RemoveNonNumeric(), CultureInfo.InvariantCulture)))
                        .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => Decimal.Parse(src.Amount.RemoveNonNumeric(), CultureInfo.InvariantCulture)))
                        .ForMember(dest => dest.Fee, opt => opt.MapFrom(src => Decimal.Parse(src.Fee.RemoveNonNumeric(), CultureInfo.InvariantCulture)));

                });

                var mapper = config.CreateMapper();
                var trades = new List<TradeData>();
                foreach (Transaction transaction in transactions)
                {
                    TradeData tradeData = mapper.Map<TradeData>(transaction);
                    try
                    {
                        context.TradeData.Add(tradeData);
                        context.SaveChanges();

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(tradeData.Pair);
                    }

                   // trades.Add(tradeData);
                }

                context.AddRange(trades);
                context.SaveChanges();
            }
        }
    }

    public class TestDBContext : DbContext
    {

        //static LoggerFactory object
        public static  ILoggerFactory loggerFactory;

        public TestDBContext(DbContextOptions<TestDBContext> options)
            : base(options)
        {
            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole(options => options.IncludeScopes = true);
            });
        }
        protected override void ConfigureConventions(
    ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>()
                .HavePrecision(18, 8);
        }

        public DbSet<TradeData> TradeData { get; set; }
    }

    public class Transaction
    {
        [Name("Date(UTC)")]
        public DateTime Date { get; set; }
        public string Pair { get; set; }
        public string Side { get; set; }
        public decimal Price { get; set; }
        public string Executed { get; set; }
        public string Amount { get; set; }
        public string Fee { get; set; }
    }

    public class TradeData
    {
        public int Id { get; set; }
        public DateTime DateUTC { get; set; }
        public string Pair { get; set; }
        public string Side { get; set; }
        public decimal Price { get; set; }
        public decimal Executed { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
    }

}