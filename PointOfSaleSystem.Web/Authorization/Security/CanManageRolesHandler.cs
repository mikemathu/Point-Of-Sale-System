using Microsoft.AspNetCore.Authorization;
using PointOfSaleSystem.Service.Services.Security;

namespace PointOfSaleSystem.Web.Authorization.Security
{
    public class CanManageRoles : IAuthorizationRequirement { }
    public class CanManageRolesHandler : AuthorizationHandler<CanManageRoles>
    {
        private readonly RoleService _privilegeService;
        public CanManageRolesHandler(RoleService roleService)
        {
            _privilegeService = roleService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanManageRoles requirement)
        {
            IEnumerable<string> userPrivileges = _privilegeService.GetUserPrivileges();

            if (userPrivileges.Contains("Can Manage Roles"))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
