namespace car_booking_service.Application.Models.Responses.UserServiceResponses
{
    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        //public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}