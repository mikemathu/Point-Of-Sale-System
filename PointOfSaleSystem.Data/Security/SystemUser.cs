using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PointOfSaleSystem.Data.Security
{
    public class SystemUser
    {
        [Key]
        public int SysUserID { get; set; }
        public string SurName { get; set; } = "";
        public string UserName { get; set; }
        public int[] UserRolesIDs { get; set; }

        [Required]
        public string Password { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public int IsEmployee { get; set; }
        public int IsSuperUser { get; set; }
        public string OtherNames { get; set; }

        [ForeignKey("RoleID")]
        public IEnumerable<Role> Role { get; set; }
        public int RoleID { get; set; }

        [ForeignKey("PrivilegeID")]
        public IEnumerable<Privilege> Privilege { get; set; }
        public int PrivilegeID { get; set; }
    }
}