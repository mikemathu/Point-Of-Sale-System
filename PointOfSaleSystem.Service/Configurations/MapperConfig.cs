using AutoMapper;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Data.Inventory;
using PointOfSaleSystem.Data.Sales;
using PointOfSaleSystem.Data.Security;
using PointOfSaleSystem.Service.Dtos.Accounts;
using PointOfSaleSystem.Service.Dtos.Inventory;
using PointOfSaleSystem.Service.Dtos.Sales;
using PointOfSaleSystem.Service.Dtos.Security;

namespace PointOfSaleSystem.Service.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            //Accounts
            CreateMap<AccountClassDto, AccountClass>().ReverseMap();
            CreateMap<AccountDto, Account>().ReverseMap();
            CreateMap<CashFlowCategoryDto, CashFlowCategory>().ReverseMap();
            CreateMap<FiscalPeriodDto, FiscalPeriod>().ReverseMap();
            CreateMap<JournalVoucherEntryDto, JournalVoucherEntry>().ReverseMap();
            CreateMap<JournalVoucherEntryDto2, JournalVoucherEntry>().ReverseMap();
            CreateMap<JournalVoucherDto, JournalVoucher>().ReverseMap();
            CreateMap<SubAccountDto, SubAccount>().ReverseMap();
            CreateMap<TransferSubAccountBalanceDto, TransferSubAccountBalance>().ReverseMap();
            CreateMap<FilterJournalVoucherDto, FilterJournalVoucher>().ReverseMap();

            //Inventory
            CreateMap<CustomerOrderDto, CustomerOrder>().ReverseMap();
            CreateMap<ItemCategoryDto, ItemCategory>().ReverseMap();
            CreateMap<ItemClassDto, ItemClass>().ReverseMap();
            CreateMap<ItemDto, Item>().ReverseMap();
            CreateMap<OtherTaxDto, OtherTax>().ReverseMap();
            CreateMap<UnitOfMeasureDto, UnitOfMeasure>().ReverseMap();
            CreateMap<VatTypeDto, VatType>().ReverseMap();
            CreateMap<FilterItemDto, FilterItem>().ReverseMap();

            //Sales
            CreateMap<OrderedItemDto, OrderedItem>().ReverseMap();
            CreateMap<PaymentDto, Payment>().ReverseMap();
            CreateMap<PaymentMethodDto, PaymentMethod>().ReverseMap();
            CreateMap<UpdateOrderedItemDto, OrderedItem>().ReverseMap();

            //Security
            CreateMap<PrivilegeDto, Privilege>().ReverseMap();
            CreateMap<RoleDto, Role>().ReverseMap();
            CreateMap<RolePrivilegeDto, RolePrivilege>().ReverseMap();
            CreateMap<SystemUser, RegisterLoginDto>().ReverseMap();
            CreateMap<SystemUser, SystemUserDto>().ReverseMap();
        }
    }
}