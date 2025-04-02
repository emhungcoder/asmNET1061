namespace foodService.Models
{
    public class Food
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public string QRCode { get; set; }
        public List<Combo> Combos { get; set; } // Add this line
    }


}
