using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using apiASM.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASM.API.Controllers
{
    [Authorize(Roles = "Customer")] 
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddCartDto dto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.CustomerId == dto.CustomerId);

            if (cart == null)
            {
                cart = new Cart { CustomerId = dto.CustomerId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var product = await _context.Products.FindAsync(dto.ProductId);
                cart.CartDetails.Add(new CartDetail
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        public class AddCartDto
        {
            public string CustomerId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        [HttpGet("get")]
        public IActionResult GetCart([FromQuery] string customerId)
        {
            var cart = _context.Carts
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Product)
                .FirstOrDefault(c => c.CustomerId == customerId);
            if (cart == null || !cart.CartDetails.Any())
            {
                return Ok(new List<CartDetail>());
            }
            return Ok(cart.CartDetails.ToList());
        }

        [HttpPost("checkout")]
        public IActionResult Checkout([FromForm] string customerId)
        {
            try
            {
                var cart = _context.Carts.FirstOrDefault(c => c.CustomerId == customerId);
                if (cart == null)
                    return BadRequest("Giỏ hàng không tồn tại.");

                var cartItems = _context.CartDetails
                    .Include(cd => cd.Product)
                    .Where(cd => cd.CartId == cart.CartId)
                    .ToList();
                if (!cartItems.Any())
                    return BadRequest("Giỏ hàng trống.");

                var newOrder = new Order
                {
                    CustomerId = customerId,
                    OrderDate = DateTime.Now,
                    TotalAmount = cartItems.Sum(ci => ci.Quantity * ci.Product.Price),
                    Status = "Đang xử lý"
                };
                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = newOrder.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                    };
                    _context.OrderDetails.Add(orderDetail);
                }
                _context.SaveChanges();

                _context.CartDetails.RemoveRange(cartItems);
                _context.SaveChanges();

                return Ok(new { message = "Đặt hàng thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        [HttpPost("remove")]
        public IActionResult RemoveFromCart([FromForm] int id)
        {
            var cartDetail = _context.CartDetails.Find(id);
            if (cartDetail != null)
            {
                _context.CartDetails.Remove(cartDetail);
                _context.SaveChanges();
            }
            return Ok(new { message = "Xóa thành công" });
        }

        [HttpPost("updatequantity")]
        public IActionResult UpdateQuantity([FromBody] UpdateCartDetailModel model)
        {
            var cartDetail = _context.CartDetails.FirstOrDefault(cd => cd.CartDetailId == model.CartDetailId);
            if (cartDetail != null)
            {
                cartDetail.Quantity = model.Quantity;
                _context.SaveChanges();
                return Ok(new { message = "Cập nhật thành công" });
            }
            return BadRequest(new { message = "Cập nhật thất bại" });
        }
    }

    public class UpdateCartDetailModel
    {
        public int CartDetailId { get; set; }
        public int Quantity { get; set; }
    }
}
