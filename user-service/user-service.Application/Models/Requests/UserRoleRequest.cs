namespace user_service.Application.Models.Requests
{
    public class UserRoleRequest
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
