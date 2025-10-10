namespace EShop.Dto.ProductModel
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public DateTime DateCreated { get; set; }
        public int StockQuantity { get; set; }
    }
}
