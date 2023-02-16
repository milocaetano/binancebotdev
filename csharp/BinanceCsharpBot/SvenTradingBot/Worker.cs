using BinanceImporter.Infrastructure;
using SvenTrading.Domain.Domain.Entities;
using SvenTrading.Infrastructure.Repositories;

namespace SvenTradingBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration configuration;
        private readonly ITradeSignalRepository tradeSignalRepository;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, ITradeSignalRepository tradeSignalRepository)
        {
            _logger = logger;
            this.configuration = configuration;
            this.tradeSignalRepository = tradeSignalRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var pendingTradeSignals = tradeSignalRepository.GetPendingTrades();

            if (pendingTradeSignals.Count > 0)
            {
                _logger.LogInformation($"Processando {pendingTradeSignals.Count} Trades");

            }
            else
            {
                _logger.LogTrace($"Não tem trades para processar");
            }
        }
    }
}