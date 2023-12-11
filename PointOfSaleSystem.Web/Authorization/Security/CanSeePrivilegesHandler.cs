using Microsoft.AspNetCore.Authorization;
using PointOfSaleSystem.Service.Services.Security;

namespace PointOfSaleSystem.Web.Authorization.Security
{
    public class CanSeePrivileges : IAuthorizationRequirement { }
    public class CanSeePrivilegesHandler : AuthorizationHandler<CanSeePrivileges>
    {
        private readonly RoleService _privilegeService;
        public CanSeePrivilegesHandler(RoleService roleService)
        {
            _privilegeService = roleService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanSeePrivileges requirement)
        {
            IEnumerable<string> userPrivileges = _privilegeService.GetUserPrivileges();

            if (userPrivileges.Contains("Can See Privileges"))
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
