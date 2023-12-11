using Microsoft.AspNetCore.Authorization;
using PointOfSaleSystem.Service.Services.Security;

namespace PointOfSaleSystem.Web.Authorization.Security
{
    public class CanManagePrivileges : IAuthorizationRequirement { }
    public class CanManagePrivilegesHandler : AuthorizationHandler<CanManagePrivileges>
    {
        private readonly RoleService _privilegeService;
        public CanManagePrivilegesHandler(RoleService roleService)
        {
            _privilegeService = roleService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanManagePrivileges requirement)
        {
            IEnumerable<string> userPrivileges = _privilegeService.GetUserPrivileges();

            if (userPrivileges.Contains("Can Manage Privileges"))
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
