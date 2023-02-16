using SvenTrading.Domain.Domain.Entities;

namespace SvenTrading.Infrastructure.Repositories
{
    public interface ITradeSignalRepository
    {
        List<TradeSignal> GetPendingTrades();
    }
}