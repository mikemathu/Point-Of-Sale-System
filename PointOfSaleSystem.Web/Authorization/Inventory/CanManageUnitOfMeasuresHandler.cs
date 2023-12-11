using Microsoft.AspNetCore.Authorization;
using PointOfSaleSystem.Service.Services.Security;

namespace PointOfSaleSystem.Web.Authorization.Inventory
{
    public class CanManageUnitOfMeasures : IAuthorizationRequirement { }
    public class CanManageUnitOfMeasuresHandler : AuthorizationHandler<CanManageUnitOfMeasures>
    {
        private readonly RoleService _privilegeService;
        public CanManageUnitOfMeasuresHandler(RoleService roleService)
        {
            _privilegeService = roleService;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanManageUnitOfMeasures requirement)
        {
            IEnumerable<string> userPrivileges = _privilegeService.GetUserPrivileges();

            if (userPrivileges.Contains("Can Manage Unit of Measures"))
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
