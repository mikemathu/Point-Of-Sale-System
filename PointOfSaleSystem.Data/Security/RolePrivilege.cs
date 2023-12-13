namespace PointOfSaleSystem.Data.Security
{
    public class RolePrivilege
    {
        public int RoleID { get; set; }
        public int PrivilegeID { get; set; }
        public Role Role { get; set; } = new Role();
        public Privilege Privilege { get; set; } = new Privilege();
        public int[] SelectedPrivilegesIDs { get; set; } = Array.Empty<int>();
    } 
}