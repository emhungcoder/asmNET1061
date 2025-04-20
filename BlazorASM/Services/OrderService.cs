using ASM.Client.Models;
using System.Net.Http.Json;

namespace ASM.Client.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId);
        Task<Order?> GetOrderByIdAsync(int orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<Order>>($"api/orders/customer/{customerId}");
            return response ?? new List<Order>();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _httpClient.GetFromJsonAsync<Order>($"api/orders/{orderId}");
        }
    }
}
