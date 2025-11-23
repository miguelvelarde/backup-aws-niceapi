namespace NiceAppApi.Sales
{
    public class SaleDetailModel
    {
        // Propiedades de Sales
        public int SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public int? ClientId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public int NoTicket { get; set; }
        public string SaleType { get; set; }
        
        // Propiedades de Products
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImage { get; set; }
        public string ProductType { get; set; }
    }
}