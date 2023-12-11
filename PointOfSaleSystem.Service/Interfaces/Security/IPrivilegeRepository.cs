using PointOfSaleSystem.Data.Security;

namespace PointOfSaleSystem.Service.Interfaces.Security
{
    public interface IPrivilegeRepository
    {
        Task<Privilege?> CreatePrivilegeAsync(Privilege privilege);
        Task<IEnumerable<Privilege>> GetAllPrivilegesAsync();
        Task<IEnumerable<Privilege>> GetRolePrivilegesAsync(int roleID);
        Task<IEnumerable<Privilege>> AddPrivilegesToRoleAsync(RolePrivilege rolePrivilege);
        Task<IEnumerable<Privilege>> DeletePrivilegesFromRoleAsync(RolePrivilege rolePrivilege);
        Task<bool> DeleteRolePrivilegeAsync(int privilegeID);
        Task<Privilege?> UpdatePrivilegeAsync(Privilege privilege);
        Task<bool> DoesPrivilegeExist(int privilegeID);
    }
}
