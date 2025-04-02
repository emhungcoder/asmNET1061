namespace foodService.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int FoodId { get; set; }
        public Food Food { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
