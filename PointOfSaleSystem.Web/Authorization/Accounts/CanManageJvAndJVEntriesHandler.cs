using Microsoft.AspNetCore.Authorization;
using PointOfSaleSystem.Service.Services.Security;

namespace PointOfSaleSystem.Web.Authorization.Accounts
{
    public class CanManageJvAndJVEntries : IAuthorizationRequirement { }
    public class CanManageJvAndJVEntriesHandler : AuthorizationHandler<CanManageJvAndJVEntries>
    {
        private readonly RoleService _privilegeService;
        public CanManageJvAndJVEntriesHandler(RoleService roleService)
        {
            _privilegeService = roleService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanManageJvAndJVEntries requirement)
        {
            IEnumerable<string> userPrivileges = _privilegeService.GetUserPrivileges();

            if (userPrivileges.Contains("Can Manage Jv And JV Entries"))
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