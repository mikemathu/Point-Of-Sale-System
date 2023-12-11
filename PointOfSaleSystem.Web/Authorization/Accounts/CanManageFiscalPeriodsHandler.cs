using Microsoft.AspNetCore.Authorization;
using PointOfSaleSystem.Service.Services.Security;

namespace PointOfSaleSystem.Web.Authorization.Accounts
{
    public class CanManageFiscalPeriods : IAuthorizationRequirement { }
    public class CanManageFiscalPeriodsHandler : AuthorizationHandler<CanManageFiscalPeriods>
    {
        private readonly RoleService _privilegeService;
        public CanManageFiscalPeriodsHandler(RoleService roleService)
        {
            _privilegeService = roleService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanManageFiscalPeriods requirement)
        {
            IEnumerable<string> userPrivileges = _privilegeService.GetUserPrivileges();

            if (userPrivileges.Contains("Can Manage FiscalPeriods"))
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