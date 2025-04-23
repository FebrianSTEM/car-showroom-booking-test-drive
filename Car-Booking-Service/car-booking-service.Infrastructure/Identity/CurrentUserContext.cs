using car_booking_service.Application.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;


namespace car_booking_service.Infrastructure.Identity
{
    public class CurrentUserContext : ICurrentUserContext
    {
        public string UserId { get; }
        public string Email { get; }
        public List<string> Roles { get; }

        public CurrentUserContext(IHttpContextAccessor accessor)
        {
            var context = accessor.HttpContext;

            if (context?.Items["UserInfo"] is JsonDocument userInfo)
            {
                var data = userInfo.RootElement.GetProperty("data");
                UserId = data.GetProperty("id").GetString();
                Email = data.GetProperty("email").GetString();
                Roles = data.GetProperty("roles").EnumerateArray().Select(x => x.GetString()).ToList();
            }
        }
    }
}
