namespace foodService.Models
{
    public class Combo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public List<Food> Foods { get; set; }
    }}
