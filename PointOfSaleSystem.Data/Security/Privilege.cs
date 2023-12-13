namespace PointOfSaleSystem.Data.Security
{
    public class Privilege
    {
        public int PrivilegeID { get; set; }
        public string PrivilegeName { get; set; } = "";
        public IEnumerable<RolePrivilege> UserRolePrivileges { get; set; } = Enumerable.Empty<RolePrivilege>();
    }
}
