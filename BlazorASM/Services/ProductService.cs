using ASM.Client.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ASM.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<Product>> GetInactiveProductsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Product>>("api/products/GetInactiveProducts");
            return response ?? new List<Product>();
        }
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Category>>("api/categories");
            return response ?? new List<Category>();
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Product>>("api/products");
            return response ?? new List<Product>();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<Product>($"api/products/{id}");
            return response;
        }

        public async Task AddAsync(Product product, MultipartFormDataContent formData)
        {
            var response = await _httpClient.PostAsync("api/products", formData);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(Product product, MultipartFormDataContent formData)
        {
            // Nếu không có formData (không cập nhật ảnh), thì vẫn tạo content từ product
            if (formData == null)
            {
                formData = new MultipartFormDataContent();
            }

            formData.Add(new StringContent(product.ProductName ?? ""), "ProductName");
            formData.Add(new StringContent(product.Price.ToString()), "Price");
            formData.Add(new StringContent(product.Quantity.ToString()), "Quantity");
            formData.Add(new StringContent(product.Color ?? ""), "Color");
            formData.Add(new StringContent(product.Size ?? ""), "Size");
            formData.Add(new StringContent(product.Description ?? ""), "Description");
            formData.Add(new StringContent(product.CategoryID.ToString()), "CategoryID");
            formData.Add(new StringContent(product.ProductID.ToString()), "ProductID"); // Cần thiết để biết cập nhật sản phẩm nào

            var response = await _httpClient.PutAsync("api/products", formData);
            response.EnsureSuccessStatusCode();
        }


        public async Task StopSellingAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/products/stop/{id}", null);
            response.EnsureSuccessStatusCode();
        }


        public async Task ActivateAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/products/activate/{id}", null);
            response.EnsureSuccessStatusCode();
        }



        public async Task<List<Product>> SearchAsync(string? searchTerm, int? categoryId, decimal? minPrice, decimal? maxPrice, string? sortOrder)
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(searchTerm)) queryParams.Add($"searchTerm={searchTerm}");
            if (categoryId.HasValue) queryParams.Add($"categoryId={categoryId}");
            if (minPrice.HasValue) queryParams.Add($"minPrice={minPrice}");
            if (maxPrice.HasValue) queryParams.Add($"maxPrice={maxPrice}");
            if (!string.IsNullOrWhiteSpace(sortOrder)) queryParams.Add($"sortOrder={sortOrder}");

            var url = "api/products/Search";
            if (queryParams.Any())
            {
                url += "?" + string.Join("&", queryParams);
            }

            var response = await _httpClient.GetFromJsonAsync<List<Product>>(url);
            return response ?? new List<Product>();
        }


    }
}
