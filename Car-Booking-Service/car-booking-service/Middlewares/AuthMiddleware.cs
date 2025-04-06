using car_booking_service.Domain.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace car_booking_service.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IConfiguration configuration, HttpClient httpClient)
        {

            var endpoint = context.GetEndpoint();
            var authorizeAttribute = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>();
            var allowAnonymous = endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null;

            if (authorizeAttribute != null && !allowAnonymous)
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(token))
                {
                    throw new HttpStatusCodeException(StatusCodes.Status401Unauthorized, "Token is missing.");
                }

                var authServiceUrl = configuration["AuthService:Url"] + "/api/Account/me";
                var request = new HttpRequestMessage(HttpMethod.Get, authServiceUrl);
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpStatusCodeException(StatusCodes.Status401Unauthorized, "Invalid or expired token.");
                }

                var json = await response.Content.ReadAsStringAsync();
                var userInfo = JsonDocument.Parse(json);
                var roles = userInfo.RootElement.GetProperty("data").GetProperty("roles").Deserialize<List<string>>();

                // Check if role matches
                var requiredRoles = authorizeAttribute.Roles?.Split(',').Select(r => r.Trim()) ?? Enumerable.Empty<string>();

                if (requiredRoles.Any() && !roles.Any(userRole => requiredRoles.Contains(userRole)))
                {
                    throw new HttpStatusCodeException(StatusCodes.Status403Forbidden, "You do not have permission to access this resource.");
                }
                context.Items["UserInfo"] = userInfo;
            }

            await _next(context);
        }
    }
}
