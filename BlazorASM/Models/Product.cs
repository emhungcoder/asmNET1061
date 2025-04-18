using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM.Client.Models
{

    public partial class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public string? TinhTrang { get; set; }
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

}
