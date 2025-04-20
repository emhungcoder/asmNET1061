using ASM.Client.Models;
using System.Net.Http.Json;

namespace ASM.Client.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrdersAsync(string? status = null, string? search = null); // <-- Thêm cho admin
        Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus); // <-- Thêm để cập nhật trạng thái
    }

    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Dành cho admin: lấy danh sách đơn hàng (có thể lọc theo trạng thái và từ khóa)
        public async Task<List<Order>> GetOrdersAsync(string? status = null, string? search = null)
        {
            var url = "api/OrderManager/orders";
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(status))
                queryParams.Add($"status={status}");
            if (!string.IsNullOrEmpty(search))
                queryParams.Add($"search={search}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetFromJsonAsync<List<Order>>(url);
            return response ?? new List<Order>();
        }

        // Lấy đơn hàng theo customerId
        public async Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<Order>>($"api/OrderManager/bycustomer?customerId={customerId}");
            return response ?? new List<Order>();
        }

        // Lấy chi tiết đơn hàng
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _httpClient.GetFromJsonAsync<Order>($"api/OrderManager/orders/{orderId}");
        }

        // Cập nhật trạng thái đơn hàng
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var formData = new MultipartFormDataContent
            {
                { new StringContent(orderId.ToString()), "id" },
                { new StringContent(newStatus), "newStatus" }
            };

            var response = await _httpClient.PostAsync("api/OrderManager/orders/update", formData);
            return response.IsSuccessStatusCode;
        }
    }
}
