namespace EShop.Data
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal SellingPrice { get; set; }
        public decimal CostPrice { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public DateTime? ExpiryDate { get; set; }
        public int StockQuantity { get; set; }
    }
}
