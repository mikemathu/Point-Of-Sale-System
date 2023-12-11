using Microsoft.AspNetCore.Authorization;
using PointOfSaleSystem.Service.Services.Security;

namespace PointOfSaleSystem.Web.Authorization.Accounts
{
    public class CanManageCashflowCategories : IAuthorizationRequirement { }
    public class CanManageCashflowCategoriesHandler : AuthorizationHandler<CanManageCashflowCategories>
    {
        private readonly RoleService _privilegeService;
        public CanManageCashflowCategoriesHandler(RoleService roleService)
        {
            _privilegeService = roleService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanManageCashflowCategories requirement)
        {
            IEnumerable<string> userPrivileges = _privilegeService.GetUserPrivileges();

            if (userPrivileges.Contains("Can Manage Cashflow Categories"))
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