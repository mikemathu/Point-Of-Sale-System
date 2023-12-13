using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Security
{
    public class SystemUser
    {
        [Key]
        public int SysUserID { get; set; }
        public string SurName { get; set; } = "";
        public string UserName { get; set; } = "";
        public int[] UserRolesIDs { get; set; } = Array.Empty<int>();

        [Required]
        public string Password { get; set; } = "";
        public DateTime DateTimeCreated { get; set; }
        public int IsEmployee { get; set; }
        public int IsSuperUser { get; set; }
        public string OtherNames { get; set; } = "";

        [ForeignKey("RoleID")]
        public IEnumerable<Role> Role { get; set; } = Enumerable.Empty<Role>();
        public int RoleID { get; set; }

        [ForeignKey("PrivilegeID")]
        public IEnumerable<Privilege> Privilege { get; set; } = Enumerable.Empty<Privilege>();
        public int PrivilegeID { get; set; }
    }
}