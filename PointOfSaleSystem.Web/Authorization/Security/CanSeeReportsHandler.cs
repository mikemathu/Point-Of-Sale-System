using Microsoft.AspNetCore.Authorization;
using PointOfSaleSystem.Service.Services.Security;

namespace PointOfSaleSystem.Web.Authorization.Security
{
    public class CanSeeReports : IAuthorizationRequirement { }
    public class CanSeeReportsHandler : AuthorizationHandler<CanSeeReports>
    {
        private readonly RoleService _privilegeService;
        public CanSeeReportsHandler(RoleService roleService)
        {
            _privilegeService = roleService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanSeeReports requirement)
        {
            IEnumerable<string> userPrivileges = _privilegeService.GetUserPrivileges();

            if (userPrivileges.Contains("Can See Reports"))
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
