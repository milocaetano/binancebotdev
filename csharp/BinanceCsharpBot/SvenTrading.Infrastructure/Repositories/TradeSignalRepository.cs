using BinanceImporter.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SvenTrading.Domain.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvenTrading.Infrastructure.Repositories
{
    public class TradeSignalRepository : ITradeSignalRepository
    {
        private readonly Func<SvenTradingContext> contextFactory;

        public TradeSignalRepository(Func<SvenTradingContext> contextFactory)
        {           
            this.contextFactory = contextFactory;
        }

        public List<TradeSignal> GetPendingTrades()
        {
            using(var tradingContext = contextFactory())
            {
                List<TradeSignal> pendingTradeSignals = (from x in tradingContext.TradeSignal where x.Status == "Pending" select x).ToList();

                return pendingTradeSignals;

            }                    
        }
    }
}
