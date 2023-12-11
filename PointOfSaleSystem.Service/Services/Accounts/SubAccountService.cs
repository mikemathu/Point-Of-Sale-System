using AutoMapper;
using PointOfSaleSystem.Data.Accounts;
using PointOfSaleSystem.Service.Dtos.Accounts;
using PointOfSaleSystem.Service.Interfaces.Accounts;
using PointOfSaleSystem.Service.Services.Exceptions;

namespace PointOfSaleSystem.Service.Services.Accounts
{
    public class SubAccountService
    {
        private readonly ISubAccountRepository _subAccountRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        public SubAccountService(ISubAccountRepository subAccountRepository, IAccountRepository accountRepository, IMapper mapper)
        {
            _subAccountRepository = subAccountRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }
        private async Task ValidateSubAccountId(int subAccountID)
        {
            if (subAccountID <= 0)
            {
                throw new ArgumentException("Invalid SubAccount Id. It must be a positive integer.");
            }
            bool doesSubAccountExist = await _subAccountRepository.DoesSubAccountExist(subAccountID);
            if (!doesSubAccountExist)
            {
                throw new ValidationRowNotFoudException($"Sub Account with Id {subAccountID} not found.");
            }
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
        public async Task CreateUpdateSubAccountAsync(SubAccountDto subAccountDto)
        {
            bool isSubAccountCreateUpdateSuccess = false;

            if (subAccountDto.SubAccountID == 0)//Create
            {
                isSubAccountCreateUpdateSuccess = await _subAccountRepository.CreateSubAccountAsync(_mapper.Map<SubAccount>(subAccountDto));
            }
            else //update
            {
                isSubAccountCreateUpdateSuccess = await _subAccountRepository.UpdateSubAccountAsync(_mapper.Map<SubAccount>(subAccountDto));
            }
            if (!isSubAccountCreateUpdateSuccess)
            {
                throw new FalseException("Could not Create/Update Account.");
            }
        }
        public async Task<IEnumerable<SubAccountDto>> GetAllSubAccountsByAccountIDAsync(int accountID)
        {
            await ValidateAccountId(accountID);
            IEnumerable<SubAccount> subAccounts = await _subAccountRepository.GetAllSubAccountsByAccountIDAsync(accountID);
            if (!subAccounts.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<SubAccountDto>>(subAccounts);
        }
        public async Task<SubAccountDto> GetSubAccountDetailsAsync(int subAccountID)
        {
            await ValidateSubAccountId(subAccountID);
            SubAccount? subAccount = await _subAccountRepository.GetSubAccountDetailsAsync(subAccountID);
            if (subAccount == null)
            {
                throw new NullException();
            }
            return _mapper.Map<SubAccountDto>(subAccount);
        }
        public async Task DeleteSubAccountAsync(int subAccountID)
        {
            await ValidateSubAccountId(subAccountID);
            bool isSubAccountDeleted = await _subAccountRepository.DeleteSubAccountAsync(subAccountID);
            if (!isSubAccountDeleted)
            {
                throw new FalseException("Could not Delete Sub Account. Pleace try again.");
            }
        }
        public async Task TransferSubAccountBalanceAsync(TransferSubAccountBalanceDto destSubAccountBalance)
        {
            double sourceSubAccountBalance = await GetSourceSubAccountBalanceAsync(destSubAccountBalance);
            bool isSubAccountBalanceTransfered = await _subAccountRepository.TransferSubAccountBalanceAsync(
                _mapper.Map<TransferSubAccountBalance>(destSubAccountBalance), sourceSubAccountBalance);
            if (!isSubAccountBalanceTransfered)
            {
                throw new FalseException("Could not transfer Balance.");
            }
        }
        private async Task<double> GetSourceSubAccountBalanceAsync(TransferSubAccountBalanceDto destSubAccountBalance)
        {
            double sourceSubAccountBalance = await _subAccountRepository.GetSourceSubAccountBalanceAsync(
              _mapper.Map<TransferSubAccountBalance>(destSubAccountBalance));
            if (sourceSubAccountBalance == 0)
            {
                throw new FalseException($"Source Sub Account has a balance of '{sourceSubAccountBalance}");
            }
            return sourceSubAccountBalance;
        }

        public async Task<IEnumerable<SubAccountDto>> GetInventorySubAccountAsync()
        {
            IEnumerable<SubAccount> inventorySubAccounts = await _subAccountRepository.GetInventorySubAccountAsync();
            if (!inventorySubAccounts.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<SubAccountDto>>(inventorySubAccounts);
        }
        public async Task<IEnumerable<SubAccountDto>> GetCostOfSalesSubAccountsAsync()
        {
            IEnumerable<SubAccount> costOfSaleSubAccount = await _subAccountRepository.GetCostOfSalesSubAccountsAsync();
            if (!costOfSaleSubAccount.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<SubAccountDto>>(costOfSaleSubAccount);
        }
        public async Task<IEnumerable<SubAccountDto>> GetIncomeSubAccountsAsync()
        {
            IEnumerable<SubAccount> incomeSubAccounts = await _subAccountRepository.GetIncomeSubAccountsAsync();
            if (!incomeSubAccounts.Any())
            {
                throw new NullException();
            }
            return _mapper.Map<IEnumerable<SubAccountDto>>(incomeSubAccounts);
        }
    }
}