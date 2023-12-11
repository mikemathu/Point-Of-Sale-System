namespace PointOfSaleSystem.Service.Dtos.Security
{
    public class SystemUserDto
    {
        public int SysUserID { get; set; }
        public int[] UserRolesIDs { get; set; }
        public string OtherNames { get; set; }
        public string Password { get; set; }
        public string SurName { get; set; }
        public string UserName { get; set; }
        public DateTime DateTimeCreated { get; set; } = DateTime.Now;
    }
    public class RegisterLoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}