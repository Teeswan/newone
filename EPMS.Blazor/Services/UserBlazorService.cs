using EPMS.Shared.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EPMS.Blazor.Services
{
    public class UserBlazorService : IUserBlazorService
    {
        private readonly HttpClient _httpClient;

        public UserBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserRoleDto>> GetAllUsersAsync()
        {
          
            var token = "your_actual_jwt_token_here";

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

          
            var response = await _httpClient.GetFromJsonAsync<List<UserRoleDto>>("api/users");
            return response ?? new List<UserRoleDto>();
        }
    }
}
