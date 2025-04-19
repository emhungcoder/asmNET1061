using ASM.Models;
using Microsoft.AspNetCore.Mvc;
using ASM.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using apiASM.Models;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepository _productRepository;
        private readonly IWebHostEnvironment _env;

        public ProductsController(ProductRepository productRepository, IWebHostEnvironment env)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _env = env;
        }

        // Chuyển đổi đối tượng Product thành ProductDTO
        private ProductDTO MapToDTO(Product p) => new ProductDTO
        {
            ProductID = p.ProductID,
            ProductName = p.ProductName,
            Price = p.Price,
            Quantity = p.Quantity,
            Color = p.Color,
            Size = p.Size,
            Description = p.Description,
            Image = p.Image,
            CategoryID = p.CategoryID,
            CategoryName = p.Category?.CategoryName,
            TinhTrang = p.TinhTrang
        };

        // Lấy tất cả sản phẩm
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllProductsAsync();
            var result = products.Select(MapToDTO).ToList();
            return Ok(result);
        }

        // Lấy sản phẩm đã ngừng bán
        [HttpGet("GetInactiveProducts")]
        public async Task<IActionResult> GetInactiveProducts()
        {
            var products = await _productRepository.GetProductsByStatusAsync("Off");
            var result = products.Select(MapToDTO).ToList();
            return Ok(result);
        }

        // Tìm kiếm sản phẩm
        [HttpGet("Search")]
        public async Task<IActionResult> SearchProducts(string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var products = await _productRepository.SearchProductsAsync(searchTerm, categoryId, minPrice, maxPrice);
            var result = products.Select(MapToDTO).ToList();
            return Ok(result);
        }

        // Lấy sản phẩm theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) return NotFound("Sản phẩm không tồn tại.");
            return Ok(MapToDTO(product));
        }

        // Thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] ProductCreateModel model)
        {
            if (model.ProductImage == null || model.ProductImage.Length == 0)
                return BadRequest(new { message = "Vui lòng chọn ảnh." });

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var imagesFolder = Path.Combine(webRoot, "images");

            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            var fileName = Path.GetFileName(model.ProductImage.FileName);
            var filePath = Path.Combine(imagesFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProductImage.CopyToAsync(stream);
            }

            var newProduct = new Product
            {
                ProductName = model.ProductName,
                Price = model.Price,
                Quantity = model.Quantity,
                Color = model.Color,
                Size = model.Size,
                Description = model.Description,
                Image = "/images/" + fileName,
                TinhTrang = "On", // Trạng thái mặc định là đang bán
                CategoryID = model.CategoryID
            };

            await _productRepository.AddProductAsync(newProduct);
            return Ok(new { message = "Thêm sản phẩm thành công!" });
        }

        // Cập nhật sản phẩm
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductUpdateModel model)
        {
            var product = await _productRepository.GetProductByIdAsync(model.ProductID);
            if (product == null)
                return NotFound(new { message = "Sản phẩm không tồn tại!" });

            product.ProductName = model.ProductName;
            product.Price = model.Price;
            product.Quantity = model.Quantity;
            product.Color = model.Color;
            product.Size = model.Size;
            product.Description = model.Description;

            if (model.ProductImage != null && model.ProductImage.Length > 0)
            {
                var fileName = Path.GetFileName(model.ProductImage.FileName);
                var filePath = Path.Combine(_env.WebRootPath, "images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProductImage.CopyToAsync(stream);
                }
                product.Image = "/images/" + fileName;
            }

            await _productRepository.UpdateProductAsync(product);
            return Ok(new { message = "Cập nhật sản phẩm thành công!" });
        }

        [HttpPost("stop/{id}")]
        public async Task<IActionResult> StopSelling(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Sản phẩm không tồn tại!" });

            product.TinhTrang = "Off";
            await _productRepository.UpdateProductAsync(product);

            return Ok(new { message = "Sản phẩm đã được ngừng bán!" });
        }

        // Kích hoạt lại sản phẩm
        [HttpPost("activate/{id}")]
        public async Task<IActionResult> Activate(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Sản phẩm không tồn tại!" });

            product.TinhTrang = "On";
            await _productRepository.UpdateProductAsync(product);

            return Ok(new { message = "Sản phẩm đã được kích hoạt lại!" });
        }



    }
}
