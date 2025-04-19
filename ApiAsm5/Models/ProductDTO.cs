namespace apiASM.Models
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public int? CategoryID { get; set; }
        public string? TinhTrang { get; set; }
    }
}
