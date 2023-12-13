using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PointOfSaleSystem.Web.Filters;
using PointOfSaleSystem.Service.Services.Security;
using PointOfSaleSystem.Service.Dtos.Security;

namespace PointOfSaleSystem.Web.ApiControllers
{
    [Route("Security")]
    [ApiController]
    [Authorize]
    [ValidateModel, HandleException]
    public class SecurityController : ControllerBase
    {
        private readonly RoleService _userRoleService;
        private readonly PrivilegeService _privilegeService;
        private readonly SystemUserService _userService;
        public SecurityController(
            RoleService userRoleService,
            PrivilegeService privilegeService,
            SystemUserService systemUserService)
        {
            _userRoleService = userRoleService;
            _privilegeService = privilegeService;
            _userService = systemUserService;
        }

        /// <summary>
        /// Login / Logout
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterLoginDto registerDto)
        {
            await _userService.RegisterUserAsync(registerDto);
            return Ok(new { message = "Success" });
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(RegisterLoginDto loginDto)
        {
            await _userService.AuthenticateUserAsync(loginDto);
            return Ok(new { message = "Success" });
        }

        [HttpGet("Logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/");
        }

        /// <summary>
        /// User Roles
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("CreateUpdateRole")]
        [Authorize("CanManageRoles")]
        public async Task<IActionResult> CreateUpdateRole(RoleDto roleDto)
        {
            RoleDto createdUpdatedUserRole = await _userRoleService.CreateUpdateRoleAsync(roleDto);
            return Ok(createdUpdatedUserRole);
        }

        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            IEnumerable<RoleDto> roles = await _userRoleService.GetAllRolesAsync();
            return Ok(roles);
        }
        [HttpPost("GetRoleDetails")]
        public async Task<IActionResult> GetRoleDetails([FromBody] int roleID)
        {
            RoleDto role = await _userRoleService.GetRoleDetailsAsync(roleID);
            return Ok(role);
        }

        [HttpPost("DeleteRole")]
        [Authorize("CanManageRoles")]
        public async Task<IActionResult> DeleteRole([FromBody] int roleID)
        {
            await _userRoleService.DeleteRoleAsync(roleID);
            return Ok(new { Responce = "Role Deleted Successfully." });
        }

        /// <summary>
        /// User Privileges
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>

        [HttpPost("CreateUpdatePrivilege")]
        [Authorize("CanManagePrivileges")]
        public async Task<IActionResult> CreateUpdatePrivilege(PrivilegeDto privilegeDto)
        {
            PrivilegeDto createdUpdatedPrivilege = await _privilegeService.CreateUpdatePrivilegeAsync(privilegeDto);
            return Ok(createdUpdatedPrivilege);
        }

        [HttpGet("GetAllPrivileges")]
        public async Task<IActionResult> GetAllPrivileges()
        {
            IEnumerable<PrivilegeDto> privileges = await _privilegeService.GetAllPrivilegesAsync();
            return Ok(privileges);
        }

        [HttpPost("GetRolePrivileges")]
        public async Task<IActionResult> GetRolePrivileges([FromBody] int roleID)
        {
            IEnumerable<PrivilegeDto> privileges = await _privilegeService.GetRolePrivilegesAsync(roleID);
            return Ok(privileges);
        }

        [HttpPost("AddPrivilegesToRole")]
        [Authorize("CanManagePrivileges")]
        public async Task<IActionResult> AddPrivilegesToRole(RolePrivilegeDto rolePrivilegeDto)
        {
            IEnumerable<PrivilegeDto> rolePrivilege = await _privilegeService.AddPrivilegesToRoleAsync(rolePrivilegeDto);
            return Ok(rolePrivilege);
        }

        [HttpPost("DeletePrivilegesFromRole")]
        [Authorize("CanManagePrivileges")]
        public async Task<IActionResult> DeletePrivilegesFromRole(RolePrivilegeDto rolePrivilegeDto)
        {
            IEnumerable<PrivilegeDto> rolePrivilege = await _privilegeService.DeletePrivilegesFromRoleAsync(rolePrivilegeDto);
            return Ok(rolePrivilege);
        }
        [HttpPost("DeleteRolePrivilege")]
        [Authorize("CanManagePrivileges")]
        public async Task<IActionResult> DeleteRolePrivilege([FromBody] int privilegeID)
        {
            await _privilegeService.DeleteRolePrivilegeAsync(privilegeID);
            return Ok(new { Responce = "Privilege Deleted Successfully." });
        }

        /// <summary>
        /// System Users
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            IEnumerable<SystemUserDto> users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost("CreateUpdateSystemUser")]
        [Authorize("CanManageUsers")]
        public async Task<IActionResult> CreateUpdateSystemUser(SystemUserDto systemUserDto)
        {
            await _userService.CreateUpdateSystemUserAsync(systemUserDto);//TODO
            return Ok(systemUserDto);
        }

        [HttpPost("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails([FromBody] int userID)
        {
            SystemUserDto users = await _userService.GetUserDetailsAsync(userID);
            return Ok(users);
        }

        /// <summary>
        /// Report Access
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>

        [HttpPost("AuthenticateAccessPassword")]
        /* [Authorize("CanSeeReports")]*/
        public async Task<IActionResult> AuthenticateAccessPassword([FromBody] string password)
        {
            await _userService.AuthenticateAccessPasswordAsync(password);
            return Ok(new { Responce = "Successfully." });
        }
    }
}