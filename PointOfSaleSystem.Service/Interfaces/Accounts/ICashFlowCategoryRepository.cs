using PointOfSaleSystem.Data.Accounts;

namespace PointOfSaleSystem.Service.Interfaces.Accounts
{
    public interface ICashFlowCategoryRepository
    {
        Task<bool> CreateCashFlowCategoryAsync(CashFlowCategory cashFlowCategory);
        Task<bool> UpdateCashFlowCategoryAsync(CashFlowCategory cashFlowCategory);
        Task<IEnumerable<CashFlowCategory>> GetActiveCashFlowCategoriesAsync();
        Task<CashFlowCategory?> GetCashFlowCategoryDetailsAsync(int cashFlowCategoryID);
        Task<bool> DeleteCashFlowCategoryAsync(int cashFlowCategoryID);
        Task<bool> DoesCashFlowCategoryExist(int cashFlowCategoryID);
    }
}