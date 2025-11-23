namespace NiceAppApi.Sales
{
    public class SaleModel
    {
        public int SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public int? ClientId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; } // Campo calculado
        public string Status { get; set; }
        public string Comments { get; set; }
        public int NoTicket { get; set; }
        public string SaleType { get; set; }
    }
}