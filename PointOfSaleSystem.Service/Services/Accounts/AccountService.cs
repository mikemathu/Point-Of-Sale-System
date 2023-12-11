using AutoMapper;
using InventoryMagement.Data.Accounts;
using InventoryManagement.Service.Dtos.Accounts;
using InventoryManagement.Service.Interfaces.Accounts;
using InventoryManagement.Service.Services.Security;

namespace PointOfSaleSystem.Service.Services.Accounts
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountClassRepository _accountClassRepository;
        private readonly IMapper _mapper;
        public AccountService(IAccountRepository accountRepository, IAccountClassRepository accountClassRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _accountClassRepository = accountClassRepository;
            _mapper = mapper;
        }
        private async Task ValidateAccountId(int accountID)
        {
            if (accountID <= 0)
            {
                throw new ArgumentException("Invalid Account Id. It must be a positive integer.");
            }
            bool doesSubAccountExist = await _accountRepository.DoesAccountExist(accountID);
            if (!doesSubAccountExist)
            {
                throw new ValidationRowNotFoudException($"Account with Id {accountID} not found.");
            }
        }
        private async Task IsAccountLockedAsync(int accountID)
        {
            bool isAccountLocked = await _accountRepository.IsAccountLockedAsync(accountID);
            if (isAccountLocked)
            {
                throw new FalseException("Cannot delete Account. You are attempting to delete a locked account.");
            }
        }
        public async Task<AccountDto> CreateUpdateAccountAsync(AccountDto accountDto)
        {
            int accountTypeID = await GetAccountTypeIdAsync(accountDto);
            bool isAccountCreateUpdateSuccess = false;
            if (accountDto.AccountNo == 0)//Create 
            {
                int accountNumber = GenerateAccountNumberByType(accountTypeID);
                isAccountCreateUpdateSuccess = await _accountRepository.CreateAccountAsync(_mapper.Map<Account>(accountDto), accountNumber);
            }
            else//Update
            {
                isAccountCreateUpdateSuccess = await _accountRepository.UpdateAccountAsync(_mapper.Map<Account>(accountDto));
            }
            if (!isAccountCreateUpdateSuccess)
            {
                throw new FalseException("Could not Create/Update Account.");
            }
            AccountDto accountDetails = await GetAccountDetailsAndAccountClassNameAsync(accountDto);
            return _mapper.Map<AccountDto>(accountDetails);
        }
        private async Task<int> GetAccountTypeIdAsync(AccountDto accountDto)
        {
            int? accountTypeID = await _accountClassRepository.GetAccountTypeIdAsync(accountDto.AccountClassID);
            if (accountTypeID == null)
            {
                throw new FalseException("Account Type not Found.");
            }
            return (int)accountTypeID;
        }
        private async Task<AccountDto> GetAccountDetailsAndAccountClassNameAsync(AccountDto accountDto)
        {
            Account? accountDetails = await _accountClassRepository.GetAccountDetailsAndAccountClassNameAsync(_mapper.Map<Account>(accountDto));
            if (accountDetails == null)
            {
                throw new FalseException("Account Details of Create Account not Found.");
            }
            return _mapper.Map<AccountDto>(accountDetails);
        }
        public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync()
        {
            IEnumerable<Account> accounts = await _accountRepository.GetAllAccountsAsync();
            if (!accounts.Any())
            {
                throw new FalseException("Accounts not Found.");
            }
            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        }
        public async Task<AccountDto> GetAccountDetailsAsync(int accountID)
        {
            await ValidateAccountId(accountID);
            Account? account = await _accountRepository.GetAccountDetailsAsync(accountID);
            if (account == null)
            {
                throw new FalseException("Account Details not Found.");
            }
            return _mapper.Map<AccountDto>(account);
        }
        public async Task DeleteAccountAsync(int accountID)
        {
            await ValidateAccountId(accountID);
            await IsAccountLockedAsync(accountID);
            bool isAccountDeleted = await _accountRepository.DeleteAccountAsync(accountID);
            if (!isAccountDeleted)
            {
                throw new FalseException("Could not Delete Account.");
            }
        }
        private int GenerateAccountNumberByType(int accountTypeID)
        {
            Random random = new Random();
            int randomNumber = random.Next(1000, 2000);

            if (accountTypeID == 2)
            {
                randomNumber = random.Next(2000, 3000);
            }
            if (accountTypeID == 3)
            {
                randomNumber = random.Next(3000, 4000);
            }
            if (accountTypeID == 4)
            {
                randomNumber = random.Next(4000, 5000);
            }
            if (accountTypeID == 5)
            {
                randomNumber = random.Next(5000, 6000);
            }
            return randomNumber;
        }
    }
}