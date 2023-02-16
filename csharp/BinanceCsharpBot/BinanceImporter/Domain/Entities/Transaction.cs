using CsvHelper.Configuration.Attributes;

namespace BinanceImporter.Domain.Entities
{
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

}