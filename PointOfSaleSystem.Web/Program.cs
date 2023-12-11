using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using PointOfSaleSystem.Repo.Accounts;
using PointOfSaleSystem.Repo.Inventory;
using PointOfSaleSystem.Repo.Sales;
using PointOfSaleSystem.Repo.Security;
using PointOfSaleSystem.Service.Configurations;
using PointOfSaleSystem.Service.Interfaces.Accounts;
using PointOfSaleSystem.Service.Interfaces.Inventory;
using PointOfSaleSystem.Service.Interfaces.Sales;
using PointOfSaleSystem.Service.Interfaces.Security;
using PointOfSaleSystem.Service.Services.Accounts;
using PointOfSaleSystem.Service.Services.Inventory;
using PointOfSaleSystem.Service.Services.Sales;
using PointOfSaleSystem.Service.Services.Security;
using PointOfSaleSystem.Web.Authorization.Accounts;
using PointOfSaleSystem.Web.Authorization.Inventory;
using PointOfSaleSystem.Web.Authorization.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc()
 .ConfigureApiBehaviorOptions(options =>
 {

     options.SuppressModelStateInvalidFilter = true;
 });

builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath = "/Security/Login";
    options.AccessDeniedPath = "/Security/AccessDenied";//TODO
});

builder.Services.AddAuthorization(options =>
{
    //Accounts
    options.AddPolicy("CanManageFiscalPeriods", policyBuilder
                => policyBuilder.AddRequirements(
                    new CanManageFiscalPeriods()
                ));

    options.AddPolicy("CanManageCashflowCategories", policyBuilder
            => policyBuilder.AddRequirements(
                new CanManageCashflowCategories()
            )); 

    options.AddPolicy("CanManageJvAndJVEntries", policyBuilder
        => policyBuilder.AddRequirements(
            new CanManageJvAndJVEntries()
        )); 
    
    options.AddPolicy("CanManageTaxes", policyBuilder
        => policyBuilder.AddRequirements(
            new CanManageTaxes()
        )); 
    
    //Inventory
    options.AddPolicy("CanManageInventory", policyBuilder
        => policyBuilder.AddRequirements(
            new CanManageInventory()
        )); 
    
    options.AddPolicy("CanManageUnitOfMeasures", policyBuilder
        => policyBuilder.AddRequirements(
            new CanManageUnitOfMeasures()
        )); 
    
    //Security
    options.AddPolicy("CanManageRoles", policyBuilder
        => policyBuilder.AddRequirements(
            new CanManageRoles()
        )); 
    
    options.AddPolicy("CanManagePrivileges", policyBuilder
        => policyBuilder.AddRequirements(
            new CanManagePrivileges()
        )); 

    options.AddPolicy("CanManageUsers", policyBuilder
        => policyBuilder.AddRequirements(
            new CanManageUsers()
        ));

    options.AddPolicy("CanSeePrivileges", policyBuilder //TODO
            => policyBuilder.AddRequirements(
                new CanSeePrivileges()
            ));

    options.AddPolicy("CanSeeUsers", policyBuilder //TODO
            => policyBuilder.AddRequirements(
                new CanSeeUsers()
            ));


    //Reports
    options.AddPolicy("CanSeeReports", policyBuilder
            => policyBuilder.AddRequirements(
                new CanSeeReports()
            )); 
});

//Accounts
builder.Services.AddSingleton<IAccountClassRepository, AccountClassRepository>();
builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
builder.Services.AddSingleton<ICashFlowCategoryRepository, CashFlowCategoryRepository>();
builder.Services.AddSingleton<IFiscalPeriodRepository, FiscalPeriodRepository>();
builder.Services.AddSingleton<IJournalVoucherEntryRepository, JournalVoucherEntryRepository>();
builder.Services.AddSingleton<IJournalVoucherRepository, JournalVoucherRepository>();
builder.Services.AddSingleton<ISubAccountRepository, SubAccountRepository>();
builder.Services.AddSingleton<ITaxRepository, TaxRepository>();
builder.Services.AddSingleton<AccountClassService>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<CashFlowCategoryService>();
builder.Services.AddSingleton<FiscalPeriodService>();
builder.Services.AddSingleton<JournalVoucherService>();
builder.Services.AddSingleton<JournalVoucherEntryService>();
builder.Services.AddSingleton<SubAccountService>();
builder.Services.AddSingleton<TaxService>();
builder.Services.AddSingleton<IAuthorizationHandler, CanManageCashflowCategoriesHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanManageFiscalPeriodsHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanManageJvAndJVEntriesHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanManageTaxesHandler>();

//Inventory
builder.Services.AddSingleton<IItemInfoRepository, ItemInfoRepository>();
builder.Services.AddSingleton<IItemRepository, ItemRepository>();
builder.Services.AddSingleton<InventoryConfig, InventoryConfigRepository>();
builder.Services.AddSingleton<IUnitofMeasureRepository, UnitofMeasureRepository>();
builder.Services.AddSingleton<ItemInfoService>();
builder.Services.AddSingleton<ItemService>();
builder.Services.AddSingleton<InventoryConfigService>();
builder.Services.AddSingleton<UnitofMeasureService>();
builder.Services.AddSingleton<IAuthorizationHandler, CanManageInventoryHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanManageUnitOfMeasuresHandler>();

//Sales
builder.Services.AddSingleton<IPaymentRepository, PaymentRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<OrderService>();
builder.Services.AddSingleton<PaymentService>();

//Security
builder.Services.AddSingleton<IRoleRepository, RoleRepository>();
builder.Services.AddSingleton<IPrivilegeRepository, PrivilegeRepository>();
builder.Services.AddSingleton<ISystemUserRepository, SystemUserRepository>();
builder.Services.AddSingleton<PrivilegeService>();
builder.Services.AddSingleton<RoleService>();
builder.Services.AddSingleton<SystemUserService>();
builder.Services.AddSingleton<IAuthorizationHandler, CanManagePrivilegesHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanManageRolesHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanManageUsersHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanSeePrivilegesHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanSeeReportsHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanSeeUsersHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseFastReport();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();