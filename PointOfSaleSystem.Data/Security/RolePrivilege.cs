namespace PointOfSaleSystem.Data.Security
{
    public class RolePrivilege
    {
        public int RoleID { get; set; }
        public int PrivilegeID { get; set; }
        public Role Role { get; set; }
        public Privilege Privilege { get; set; }
        public int[] SelectedPrivilegesIDs { get; set; }
    }
}