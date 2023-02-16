using AutoMapper;
using BinanceImporter.Domain.Entities;
using BinanceImporter.Extensions;
using BinanceImporter.Infrastructure;
using System;
using System.Globalization;


namespace BinanceImporter.App
{
    public class BinanceCsvImporter
    {
        private readonly TestDBContext testDBContext;
        private readonly ICsvExtrator csvExtrator;

        public BinanceCsvImporter(TestDBContext testDBContext, ICsvExtrator csvExtrator)
        {
            this.testDBContext = testDBContext;
            this.csvExtrator = csvExtrator;
        }

        public void Import(string csvfile)
        {            
            List<Transaction> transactions = csvExtrator.GetObjects<Transaction>(csvfile);
            List<TradeData> listaDeTradeDatas = GetTradeDataListFromTransations(transactions);
            //  DateTime MaxDate = testDBContext.TradeData.Max(x => x.DateUTC);

            var filtredTrades = listaDeTradeDatas;
            if (testDBContext.TradeData.Any())
            {
                DateTime MaxDate = testDBContext.TradeData.Max(x => x.DateUTC);
                filtredTrades = (from x in listaDeTradeDatas
                                 where x.DateUTC > MaxDate
                                 select x).ToList();
            }   

            foreach (TradeData tradeData in filtredTrades)
            {               
                try
                {
                    testDBContext.TradeData.Add(tradeData);
                    testDBContext.SaveChanges();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(tradeData.Pair);
                }            
            }
        }

        private List<TradeData> GetTradeDataListFromTransations(List<Transaction> transactions)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Transaction, TradeData>()
                    .ForMember(dest => dest.DateUTC, opt => opt.MapFrom(src => src.Date))
                    .ForMember(dest => dest.Executed, opt => opt.MapFrom(src => Decimal.Parse(src.Executed.RemoveNonNumeric(), CultureInfo.InvariantCulture)))
                    .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => Decimal.Parse(src.Amount.RemoveNonNumeric(), CultureInfo.InvariantCulture)))
                    .ForMember(dest => dest.Fee, opt => opt.MapFrom(src => Decimal.Parse(src.Fee.RemoveNonNumeric(), CultureInfo.InvariantCulture)));

            });

            var mapper = config.CreateMapper();

            List<TradeData> tradeDataList = mapper.Map<List<TradeData>>(transactions);

            return tradeDataList;
        }
    }
}

