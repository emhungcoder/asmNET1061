namespace foodService.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalAmount { get; set; }
    }
}
