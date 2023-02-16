namespace SvenTrading.Domain.Entities
{
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