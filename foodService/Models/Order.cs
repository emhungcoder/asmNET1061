namespace foodService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } // "Đã giao", "Chưa giao", "Đang giao"
        public List<OrderDetail> OrderDetails { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
