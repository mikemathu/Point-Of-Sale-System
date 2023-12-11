using PointOfSaleSystem.Data.Security;

namespace PointOfSaleSystem.Service.Interfaces.Security
{
    public interface IRoleRepository
    {
        Task<Role?> CreateRoleAsync(Role role);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleDetailsAsync(int roleID);
        IEnumerable<string> GetUserPrivilege(int userID);
        Task<Role?> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int roleID);
        Task<bool> DoesRoleExist(int roleID);
    }
}
