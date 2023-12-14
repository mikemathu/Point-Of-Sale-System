using PointOfSaleSystem.Data.Accounts;

namespace PointOfSaleSystem.Service.Interfaces.Accounts
{
    public interface ISubAccountRepository
    {
        Task<bool> CreateSubAccountAsync(SubAccount subAccount);
        Task<IEnumerable<SubAccount>> GetAllSubAccountsByAccountIDAsync(int accountID);
        Task<SubAccount?> GetSubAccountDetailsAsync(int subAccountID);
        public abstract Task<bool> UpdateSubAccountAsync(SubAccount subAccount);
        Task<bool> DeleteSubAccountAsync(int subAccountID);
        Task<double> GetSourceSubAccountBalanceAsync(TransferSubAccountBalance sourceSubAccount);//TODO: combine this method to GetSourceSubAccountBalanceAsync(int sourceSubAccountID). From callers to its implementation
        Task<double> GetSourceSubAccountBalanceAsync(int sourceSubAccountID);
        Task<bool> TransferSubAccountBalanceAsync(TransferSubAccountBalance destSubAccountBalance, double sourceSubAccountBalance);

        //Inventoty project
        Task<IEnumerable<SubAccount>> GetInventorySubAccountAsync();
        Task<IEnumerable<SubAccount>> GetCostOfSalesSubAccountsAsync();
        Task<IEnumerable<SubAccount>> GetIncomeSubAccountsAsync();
        Task<bool> DoesSubAccountExist(int subAccountID);
    }
}
