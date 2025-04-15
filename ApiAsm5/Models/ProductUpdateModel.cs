using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM5.Models
{
    [NotMapped]
    public class ProductUpdateModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IFormFile? ProductImage { get; set; }
    }
}
