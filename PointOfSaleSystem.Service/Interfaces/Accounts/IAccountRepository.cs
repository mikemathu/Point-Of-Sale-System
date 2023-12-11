using PointOfSaleSystem.Data.Accounts;

namespace PointOfSaleSystem.Service.Interfaces.Accounts
{
    public interface IAccountRepository
    {
        Task<bool> CreateAccountAsync(Account account, int accountNumber);      //public abstract implicitly
        void AddAccountDetailsAsync(Account accountDetail);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account?> GetAccountDetailsAsync(int accountID);
        Task<bool> UpdateAccountAsync(Account account);
        Task<bool> DeleteAccountAsync(int accountID);
        Task<bool> DoesAccountExist(int accountID);
        Task<bool> IsAccountLockedAsync(int accountID);

    }
}
