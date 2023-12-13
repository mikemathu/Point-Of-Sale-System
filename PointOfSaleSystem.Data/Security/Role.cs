namespace PointOfSaleSystem.Data.Security
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = "";
        public string Description { get; set; } = "";
        public IEnumerable<RolePrivilege> UserRolePrivileges { get; set; } = Enumerable.Empty<RolePrivilege>();
    }
}