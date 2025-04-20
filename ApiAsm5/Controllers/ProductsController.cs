using ASM.Models;
using Microsoft.AspNetCore.Mvc;
using ASM.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using apiASM.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ASM.API.Controllers
{
   

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepository _productRepository;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

            public ProductsController(ProductRepository productRepository, IWebHostEnvironment env, ApplicationDbContext context)
            {
                _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
                _env = env;
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            // DTO Mapping
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
        [Authorize(Roles = "Manager, Employee")]
        [HttpGet("GetInactiveProducts")]
        public async Task<IActionResult> GetInactiveProducts()
        {
            var products = await _productRepository.GetProductsByStatusAsync("Off");
            var result = products.Select(MapToDTO).ToList();
            return Ok(result);
        }

        // Tìm kiếm sản phẩm
        [HttpGet("Search")]
        public async Task<IActionResult> SearchProducts(
    string? searchTerm,
    int? categoryId,
    decimal? minPrice,
    decimal? maxPrice,
    string? sortOrder)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.TinhTrang == "On") // chỉ lấy sản phẩm đang bán
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(p => p.ProductName.Contains(searchTerm));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryID == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // Sắp xếp theo giá
            if (!string.IsNullOrEmpty(sortOrder))
            {
                if (sortOrder.ToLower() == "asc")
                    query = query.OrderBy(p => p.Price);
                else if (sortOrder.ToLower() == "desc")
                    query = query.OrderByDescending(p => p.Price);
            }

            var result = await query.Select(p => new ProductDTO
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
                CategoryName = p.Category.CategoryName,
                TinhTrang = p.TinhTrang
            }).ToListAsync();

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

            // Lấy sản phẩm theo danh mục
            [HttpGet("bycategory/{categoryId}")]
            public async Task<IActionResult> GetProductsByCategory(int categoryId)
            {
                var products = await _context.Products
                    .Where(p => p.CategoryID == categoryId && p.TinhTrang == "On")
                    .Include(p => p.Category)
                    .Select(p => new ProductDTO
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
                        CategoryName = p.Category.CategoryName,
                        TinhTrang = p.TinhTrang
                    })
                    .ToListAsync();

                return Ok(products);
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
                    TinhTrang = "On",
                    CategoryID = model.CategoryID
                };

                await _productRepository.AddProductAsync(newProduct);
                return Ok(new { message = "Thêm sản phẩm thành công!" });
            }

        // Cập nhật sản phẩm
        [Authorize(Roles = "Manager, Employee")]
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

        // Ngừng bán sản phẩm
        [Authorize(Roles = "Manager, Employee")]
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
        [Authorize(Roles = "Manager, Employee")]
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

        // API lấy tất cả sản phẩm đang bán (không cần quá chi tiết)
        [HttpGet("get-all")]
        public IActionResult GetAllProduct()
        {
            var products = _context.Products
                .Where(p => p.TinhTrang == "On")
                .Select(p => new
                {
                    p.ProductID,
                    p.ProductName,
                    p.Price,
                    p.Image,
                    p.Description
                })
                .ToList();

                return Ok(products);
            }
        }
    }
