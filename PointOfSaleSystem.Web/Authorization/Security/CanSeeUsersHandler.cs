using Microsoft.AspNetCore.Authorization;
using PointOfSaleSystem.Service.Services.Security;

namespace PointOfSaleSystem.Web.Authorization.Security
{
    public class CanSeeUsers : IAuthorizationRequirement { }
    public class CanSeeUsersHandler : AuthorizationHandler<CanSeeUsers>
    {
        private readonly RoleService _privilegeService;
        public CanSeeUsersHandler(RoleService roleService)
        {
            _privilegeService = roleService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanSeeUsers requirement)
        {
            IEnumerable<string> userPrivileges = _privilegeService.GetUserPrivileges();

            if (userPrivileges.Contains("Can See Users"))
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
