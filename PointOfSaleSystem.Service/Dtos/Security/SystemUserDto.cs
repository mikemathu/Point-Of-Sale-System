namespace PointOfSaleSystem.Service.Dtos.Security
{
    public class SystemUserDto
    {
        public int SysUserID { get; set; }
        public int[] UserRolesIDs { get; set; } = new int[0];
        public string OtherNames { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string SurName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public DateTime DateTimeCreated { get; set; } = DateTime.Now;
    }
    public class RegisterLoginDto
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}