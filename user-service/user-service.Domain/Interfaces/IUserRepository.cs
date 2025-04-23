using user_service.Domain.Entities;

namespace user_service.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<List<User>> GetByIdsAsync(List<Guid> ids);
        Task<User> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<IList<string>> GetUserRolesAsync(User user);
    }
}
