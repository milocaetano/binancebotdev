using System.ComponentModel.DataAnnotations.Schema;


namespace SvenTrading.Domain.Domain.Entities
{
    public class TradeSignal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string TradeId { get; set; }
        public decimal Entry1 { get; set; }
        public decimal Entry2 { get; set; }
        public decimal? Target1 { get; set; }
        public decimal? Target2 { get; set; }
        public decimal? Target3 { get; set; }
        public decimal? Target4 { get; set; }
        public decimal? Target5 { get; set; }
        public decimal? Target6 { get; set; }
        public decimal? Target7 { get; set; }
        public decimal? Target8 { get; set; }
        public decimal? Target9 { get; set; }
        public decimal StopLoss { get; set; }
        public string Pair { get; set; }
        public string Position { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
