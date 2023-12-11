using PointOfSaleSystem.Data.Accounts;

namespace PointOfSaleSystem.Service.Interfaces.Accounts
{
    public interface IAccountClassRepository
    {
        Task<bool> CreateAccountClassAsync(AccountClass accountClass);
        Task<IEnumerable<AccountClass>> GetAllAccountClassesAsync();
        Task<Account?> GetAccountDetailsAndAccountClassNameAsync(Account account);
        Task<int?> GetAccountTypeIdAsync(int accountClassID);
    }
}
