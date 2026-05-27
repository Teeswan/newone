using EPMS.Shared.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace EPMS.Blazor.Services
{
    public class NotificationBlazorService : INotificationBlazorService
    {
        private readonly HttpClient _httpClient;

        public NotificationBlazorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            return await _httpClient.GetFromJsonAsync<List<NotificationDto>>($"api/notification/user/{userId}") ?? new List<NotificationDto>();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _httpClient.GetFromJsonAsync<int>($"api/notification/unread-count/{userId}");
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            await _httpClient.PutAsync($"api/notification/mark-as-read/{notificationId}", null);
        }
    }
}
