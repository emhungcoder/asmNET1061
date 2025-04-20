using System.Net.Http.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using ASM.Client.Models;

namespace ASM.Client.Service
{
    public class EmployeeService
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "api/Employee";

        public EmployeeService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            try
            {
                var response = await _http.GetAsync(BaseUrl);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();
                return data ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi khi lấy danh sách nhân viên: " + ex.Message);
                return new();
            }
        }

        public async Task<bool> AddEmployeeAsync(EmployeeDto employee, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(employee.FullName) || string.IsNullOrWhiteSpace(employee.Email) ||
                    string.IsNullOrWhiteSpace(employee.Address) || string.IsNullOrWhiteSpace(employee.PhoneNumber) || string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("❌ Dữ liệu nhân viên không hợp lệ.");
                    return false;
                }

                var formData = new MultipartFormDataContent
                {
                    { new StringContent(employee.FullName), "FullName" },
                    { new StringContent(employee.Email), "Email" },
                    { new StringContent(employee.Address), "Address" },
                    { new StringContent(employee.PhoneNumber), "PhoneNumber" },
                    { new StringContent(password), "Password" }
                };

                var response = await _http.PostAsync($"{BaseUrl}/add", formData);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi thêm nhân viên: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateRoleAsync(string userId, string newRole)
        {
            try
            {
                // Đảm bảo thuộc tính đúng tên như API yêu cầu
                var payload = new { UserId = userId };
                var response = await _http.PostAsJsonAsync($"{BaseUrl}/updaterole", payload);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi cập nhật vai trò: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Kích hoạt hoặc ngừng hoạt động tài khoản.
        /// </summary>
        /// <param name="userId">ID của nhân viên</param>
        /// <param name="activate">true = kích hoạt, false = ngừng hoạt động</param>
        public async Task<bool> SetActiveStatusAsync(string userId, bool activate)
        {
            try
            {
                var endpoint = activate ? $"{BaseUrl}/activate" : $"{BaseUrl}/deactivate";
                var payload = new { UserId = userId }; // Đúng với API dùng [FromBody]
                var response = await _http.PostAsJsonAsync(endpoint, payload);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                var action = activate ? "kích hoạt" : "ngừng hoạt động";
                Console.WriteLine($"❌ Lỗi khi {action} nhân viên: {ex.Message}");
                return false;
            }
        }

        public Task<bool> DeactivateAsync(string userId) => SetActiveStatusAsync(userId, false);
        public Task<bool> ActivateAsync(string userId) => SetActiveStatusAsync(userId, true);
    }
}
