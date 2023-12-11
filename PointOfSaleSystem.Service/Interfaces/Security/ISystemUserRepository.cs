using PointOfSaleSystem.Data.Security;

namespace PointOfSaleSystem.Service.Interfaces.Security
{
    public interface ISystemUserRepository
    {
        Task<SystemUser?> AuthenticateUserAsync(SystemUser systemUser);
        Task<SystemUser?> RegisterUserAsync(SystemUser systemUser);
        Task<IEnumerable<SystemUser>> GetAllUsersAsync();
        Task<SystemUser?> GetUserDetailsAsync(int userID);
        Task<bool> CreateSystemUserAsync(SystemUser user);
        Task<bool> UpdateSystemUserAsync(SystemUser user);
        Task<bool> DoesUserExist(int systemUserID);
        Task<bool> AuthenticateAccessPasswordAsync(int userID, string password);
    }
}
