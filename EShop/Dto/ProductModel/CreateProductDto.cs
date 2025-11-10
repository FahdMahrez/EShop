namespace EShop.Dto.ProductModel
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
