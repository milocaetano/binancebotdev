using System.Globalization;
using AutoMapper;
using BinanceImporter.App;
using BinanceImporter.Config;
using BinanceImporter.Domain.Entities;
using BinanceImporter.Extensions;
using BinanceImporter.Infrastructure;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BinanceImporter
{

    class Program
    {

        static void Main(string[] args)
        {

            if(args.Length == 0)
            {
                Console.WriteLine("Param File is required");

                return;
            }
            
            string file = args[0];
            
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            ImporterConfig ImporterConfig = configuration.GetSection("ImporterConfig").Get<ImporterConfig>();

            try
            {
                using (var context = new TestDBContext(ImporterConfig))
                {
                    var csvExtrator = new CsvExtrator();

                    BinanceCsvImporter importer = new BinanceCsvImporter(context, csvExtrator);

                    importer.Import(file);
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }                
        }       
    }
}