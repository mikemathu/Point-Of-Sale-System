namespace PointOfSaleSystem.Service.Dtos.Security
{
    public class RoleDto
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
