namespace PointOfSaleSystem.Service.Dtos.Security
{
    public class RolePrivilegeDto
    {
        public int RoleID { get; set; }
        public int[] SelectedPrivilegesIDs { get; set; } = new int[0];
    }
}
