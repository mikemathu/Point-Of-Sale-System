using Microsoft.AspNetCore.Authorization;
using PointOfSaleSystem.Service.Services.Security;

namespace PointOfSaleSystem.Web.Authorization.Accounts
{
    public class CanManageTaxes : IAuthorizationRequirement { }
    public class CanManageTaxesHandler : AuthorizationHandler<CanManageTaxes>
    {
        private readonly RoleService _privilegeService;
        public CanManageTaxesHandler(RoleService roleService)
        {
            _privilegeService = roleService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanManageTaxes requirement)
        {
            IEnumerable<string> userPrivileges = _privilegeService.GetUserPrivileges();

            if (userPrivileges.Contains("Can Manage Taxes"))
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
