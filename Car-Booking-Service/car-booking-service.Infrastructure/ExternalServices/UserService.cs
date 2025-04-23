using System.Net.Http.Json;
using System.Text.Json;
using car_booking_service.Application.Models.Responses.UserServiceResponses;
using car_booking_service.Application.Services.Interfaces;
using car_booking_service.Domain.Common;

namespace car_booking_service.Infrastructure.ExternalServices
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserResponse>> GetUserByIds(List<string> request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/Account/by-ids", request);
                var rawString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
                    PropertyNameCaseInsensitive = true
                };

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to fetch users from external service: {rawString}");
                }

                var result = JsonSerializer.Deserialize<StandardResponse<List<UserResponse>>>(rawString, options);

                return result?.Data ?? new List<UserResponse>();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
