using ASM.Client.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace ASM.Client.Services
{
    public class CartService
    {
        private readonly HttpClient _http;

        public CartService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        // Thêm sản phẩm vào giỏ hàng
        public async Task AddToCartAsync(string customerId, int productId, int quantity)
        {
            var response = await _http.PostAsJsonAsync("api/cart/add", new
            {
                ProductId = productId,
                Quantity = quantity,
                CustomerId = customerId
            });

            response.EnsureSuccessStatusCode();
        }

        // Lấy giỏ hàng của người dùng
        public async Task<List<CartDetail>> GetCartAsync(string customerId)
        {
            var response = await _http.GetFromJsonAsync<List<CartDetail>>($"api/cart/get?customerId={customerId}");
            return response ?? new List<CartDetail>();
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        public async Task UpdateQuantityAsync(int cartDetailId, int quantity)
        {
            var response = await _http.PostAsJsonAsync("api/cart/updatequantity", new
            {
                CartDetailId = cartDetailId,
                Quantity = quantity
            });

            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> RemoveFromCartAsync(int cartDetailId)
        {
            var content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("id", cartDetailId.ToString())
            });

            var response = await _http.PostAsync("api/cart/remove", content);
            return response.IsSuccessStatusCode;
        }

        // Thanh toán giỏ hàng
        public async Task<bool> CheckoutAsync(string customerId)
        {
            var content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("customerId", customerId)
            });

            var response = await _http.PostAsync("api/cart/checkout", content);
            return response.IsSuccessStatusCode;
        }

    }
}
