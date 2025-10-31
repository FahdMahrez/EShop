namespace EShop.Dto
{
    public class AddProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
    }
}
