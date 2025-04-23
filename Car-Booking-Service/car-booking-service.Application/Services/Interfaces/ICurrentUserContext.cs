namespace car_booking_service.Application.Services.Interfaces
{
    public interface ICurrentUserContext
    {
        string UserId { get; }
        string Email { get; }
        List<string> Roles { get; }
    }
}