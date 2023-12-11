using AutoMapper;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Dtos.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Accounts
{
    public class CashFlowCategoryService
    {
        private readonly ICashFlowCategoryRepository _cashFlowCategoryRepository;
        private readonly IMapper _mapper;

        public CashFlowCategoryService(ICashFlowCategoryRepository cashFlowCategoryRepository, IMapper mapper)
        {
            _cashFlowCategoryRepository = cashFlowCategoryRepository;
            _mapper = mapper;
        }
        private async Task ValidateCashFlowCategoryId(int cashFlowCategoryID)
        {
            if (cashFlowCategoryID <= 0)
            {
                throw new ArgumentException("Invalid CashFlowCategory Id. It must be a positive integer.");
            }
            bool doesCashFlowCategoryExist = await _cashFlowCategoryRepository.DoesCashFlowCategoryExist(cashFlowCategoryID);
            if (!doesCashFlowCategoryExist)
            {
                throw new ValidationRowNotFoudException($"CashFlow Category with Id {cashFlowCategoryID} not found.");
            }
        }
        public async Task CreateUpdateCashFlowCategoryAsync(CashFlowCategoryDto cashFlowCategory)
        {
            bool isCashFlowCategoryCreatUpdateSuccess = false;
            if (cashFlowCategory.CashFlowCategoryID == 0)//Create
            {
                isCashFlowCategoryCreatUpdateSuccess = await _cashFlowCategoryRepository.CreateCashFlowCategoryAsync(
               _mapper.Map<CashFlowCategory>(cashFlowCategory));
            }
            else//update
            {
                isCashFlowCategoryCreatUpdateSuccess = await _cashFlowCategoryRepository.UpdateCashFlowCategoryAsync(
               _mapper.Map<CashFlowCategory>(cashFlowCategory));
            }
            if (!isCashFlowCategoryCreatUpdateSuccess)
            {
                throw new FalseException("Could not Create/Update CashFlow Category. Try again later.");
            }
        }
        public async Task<IEnumerable<CashFlowCategoryDto>> GetActiveCashFlowCategoriesAsync()
        {
            IEnumerable<CashFlowCategory> cashFlowCategories = await _cashFlowCategoryRepository.GetActiveCashFlowCategoriesAsync();
            if (!cashFlowCategories.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<CashFlowCategoryDto>>(cashFlowCategories);
        }
        public async Task<CashFlowCategoryDto> GetCashFlowCategoryDetailsAsync(int cashFlowCategoryID)
        {
            await ValidateCashFlowCategoryId(cashFlowCategoryID);
            CashFlowCategory? cashFlowCategoryDetails = await _cashFlowCategoryRepository.GetCashFlowCategoryDetailsAsync(cashFlowCategoryID);
            if (cashFlowCategoryDetails == null)
            {
                throw new NullException();
            }
            return _mapper.Map<CashFlowCategoryDto>(cashFlowCategoryDetails);
        }
        public async Task DeleteCashFlowCategoryAsync(int cashFlowCategoryID)
        {
            await ValidateCashFlowCategoryId(cashFlowCategoryID);
            bool isCashFlowCategoryDeleted = await _cashFlowCategoryRepository.DeleteCashFlowCategoryAsync(cashFlowCategoryID);
            if (!isCashFlowCategoryDeleted)
            {
                throw new FalseException("Could not Delete Cashflow Category.Try again later.");
            }
        }
    }
}
