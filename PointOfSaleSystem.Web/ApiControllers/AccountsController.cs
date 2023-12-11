using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.Service.Dtos.Accounts;
using PointOfSaleSystem.Service.Dtos.Inventory;
using PointOfSaleSystem.Service.Services.Accounts;
using PointOfSaleSystem.Web.Filters;

namespace PointOfSaleSystem.Web.ApiControllers
{
    [Route("Accounts")]
    [ApiController]
    [Authorize]
    [ValidateModel, HandleException]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly SubAccountService _subAccountService;
        private readonly AccountClassService _accountClassService;
        private readonly JournalVoucherService _journalVoucherService;
        private readonly JournalVoucherEntryService _journalVoucherEntryService;
        private readonly CashFlowCategoryService _cashFlowCategoryService;
        private readonly FiscalPeriodService _fiscalPeriodService;
        private readonly TaxService _taxService;
        public AccountsController(
            AccountService accountsService,
            SubAccountService subAccountService,
            AccountClassService accountClassService,
            JournalVoucherService journalVoucherService,
            JournalVoucherEntryService journalVoucherEntryService,
            CashFlowCategoryService cashFlowCategoryService,
            FiscalPeriodService fiscalPeriodService,
            TaxService taxService)
        {
            _accountService = accountsService;
            _subAccountService = subAccountService;
            _accountClassService = accountClassService;
            _journalVoucherService = journalVoucherService;
            _journalVoucherEntryService = journalVoucherEntryService;
            _cashFlowCategoryService = cashFlowCategoryService;
            _fiscalPeriodService = fiscalPeriodService;
            _taxService = taxService;
        }

        /// <summary>
        /// Accounts
        /// </summary>
        /// <param name="accountDto"></param>
        /// <returns></returns>

        [HttpPost("CreateUpdateAccount")]
        public async Task<IActionResult> CreateUpdateAccount(AccountDto accountDto)
        {
            AccountDto accountDetails = await _accountService.CreateUpdateAccountAsync(accountDto);
            return Ok(accountDetails);
        }

        [HttpGet("GetAllAccounts")]
        public async Task<IActionResult> GetAllAccounts()
        {
            IEnumerable<AccountDto> accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        [HttpPost("GetAccountDetails")]
        public async Task<IActionResult> GetAccountDetails([FromBody] int accountID)
        {
            AccountDto accountDetails = await _accountService.GetAccountDetailsAsync(accountID);
            return Ok(accountDetails);
        }

        [HttpPost("DeleteAccount")]
        [Authorize("CanDeleteAccountPolic")]
        public async Task<IActionResult> DeleteAccount([FromBody] int accountID)
        {
            await _accountService.DeleteAccountAsync(accountID);
            return Ok(new { Responce = "Account Deleted Successfully." });
        }

        /// <summary>
        /// Sub Account
        /// </summary>
        /// <param name="subAccountDto"></param>
        /// <returns></returns>
        [HttpPost("CreateUpdateSubAccount")]
        public async Task<IActionResult> CreateUpdateSubAccount(SubAccountDto subAccountDto)
        {
            await _subAccountService.CreateUpdateSubAccountAsync(subAccountDto);//TODO: Create and return created SubAccount for convenience of api clients
            return Ok(subAccountDto);
        }

        [HttpPost("GetAllSubAccountsByAccountID")]
        public async Task<IActionResult> GetAllSubAccountsByAccountID([FromBody] int accountID)
        {
            IEnumerable<SubAccountDto> accountSubAccounts = await _subAccountService.GetAllSubAccountsByAccountIDAsync(accountID);
            return Ok(accountSubAccounts);
        }
        [HttpPost("GetSubAccountDetails")]
        public async Task<IActionResult> GetSubAccountDetails([FromBody] int subAccountID)
        {
            SubAccountDto subAccountDetails = await _subAccountService.GetSubAccountDetailsAsync(subAccountID);
            return Ok(subAccountDetails);
        }

        [HttpPost("DeleteSubAccount")]
        public async Task<IActionResult> DeleteSubAccount([FromBody] int subAccountID)
        {
            await _subAccountService.DeleteSubAccountAsync(subAccountID);
            return Ok(new { Responce = "Sub Account Deleted Successfully." });
        }

        [HttpPost("TransferSubAccountBalance")]
        public async Task<IActionResult> TransferSubAccountBalance(TransferSubAccountBalanceDto destSubAccountBalance)
        {
            await _subAccountService.TransferSubAccountBalanceAsync(destSubAccountBalance);
            return Ok(new { Responce = "Balance tranfered Successfully" });
        }

        /// <summary>
        /// Account Class
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("CreateAccountClass")]
        public async Task<IActionResult> CreateAccountClass(AccountClassDto accountClassDto)
        {
            await _accountClassService.CreateAccountClassAsync(accountClassDto);//TODO: Create and return created Account class for convenience of api clients
            return Ok(accountClassDto);
        }

        [HttpGet("GetAllAccountClasses")]
        public async Task<IActionResult> GetAllAccountClasses()
        {
            IEnumerable<AccountClassDto> accountClasses = await _accountClassService.GetAllAccountClassesAsync();
            return Ok(accountClasses);
        }

        /// <summary>
        /// JournalVoucher
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("CreateUpdateJournalVoucher")]
        [Authorize("CanManageJvAndJVEntries")]
        public async Task<IActionResult> CreateUpdateJournalVoucher(JournalVoucherDto journalVoucherDto)
        {
            JournalVoucherDto journalVoucher = await _journalVoucherService.CreateUpdateJournalVoucherAsync(journalVoucherDto);
            return Ok(journalVoucher);
        }

        [HttpPost("GetJournalVoucherDetails")]
        public async Task<IActionResult> GetJournalVoucherDetails([FromBody] int journalVoucherID)
        {
            JournalVoucherDto JournalVoucherDetails = await _journalVoucherService.GetJournalVoucherDetailsAsync(journalVoucherID);
            return Ok(JournalVoucherDetails);
        }

        [HttpPost("FilterJournalVouchers")]
        public async Task<IActionResult> FilterJournalVouchers(FilterJournalVoucherDto filterJournalVoucherDto)
        {
            IEnumerable<JournalVoucherDto> journalVouchers = await _journalVoucherService.FilterJournalVouchersAsync(filterJournalVoucherDto);
            return Ok(journalVouchers);
        }

        [HttpPost("PostJournalVoucher")]
        [Authorize("CanManageJvAndJVEntries")]
        public async Task<IActionResult> PostJournalVoucher([FromBody] int journalVoucherID)
        {
            await _journalVoucherService.PostJournalVoucherAsync(journalVoucherID);
            return Ok(new { Response = "Journal Voucher Posted Successfuly" });
        }

        [HttpPost("UnPostJournalVoucher")]
        [Authorize("CanManageJvAndJVEntries")]
        public async Task<IActionResult> UnPostJournalVoucher([FromBody] int journalVoucherID)
        {
            await _journalVoucherService.UnPostJournalVoucherAsync(journalVoucherID);
            return Ok(new { Response = "Journal Voucher UnPosted Successfuly" });
        }

        [HttpPost("DeleteJournalVoucher")]
        [Authorize("CanManageJvAndJVEntries")]
        public async Task<IActionResult> DeleteJournalVoucher([FromBody] int journalVoucherID)
        {
            await _journalVoucherService.DeleteJournalVoucherAsync(journalVoucherID);
            return Ok(new { Response = "Journal Voucher deleted successfully." });
        }

        /// <summary>
        /// JournalVoucher Entry
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("CreateUpdateEntry")]
        [Authorize("CanManageJvAndJVEntries")]
        public async Task<IActionResult> CreateUpdateEntry(JournalVoucherEntryDto2 journalVoucherEntryDto)
        {
            await _journalVoucherEntryService.CreateUpdateEntryAsync(journalVoucherEntryDto);//TODO
            return Ok(journalVoucherEntryDto);
        }

        [HttpPost("GetJournalVoucherEntries")]
        public async Task<IActionResult> GetJournalVoucherEntries([FromBody] int journalVoucherID)
        {
            IEnumerable<JournalVoucherEntryDto> journalVoucherEntries = await _journalVoucherEntryService.GetJournalVoucherEntriesAsync(journalVoucherID);
            return Ok(journalVoucherEntries);
        }

        [HttpPost("GetJournalVoucherEntryDetails")]
        public async Task<IActionResult> GetJournalVoucherEntryDetails([FromBody] int accountEntryID)
        {
            JournalVoucherEntryDto accountEntry = await _journalVoucherEntryService.GetJournalVoucherEntryDetailsAsync(accountEntryID);
            return Ok(accountEntry);
        }

        [HttpPost("DeleteEntry")]
        [Authorize("CanManageJvAndJVEntries")]
        public async Task<IActionResult> DeleteEntry([FromBody] int accountEntryID)
        {
            await _journalVoucherEntryService.DeleteEntryAsync(accountEntryID);
            return Ok(new { Response = "Account Entry deleted successfully." });
        }

        /// <summary>
        /// CashFlow Category
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("CreateUpdateCashFlowCategory")]
        [Authorize("CanManageCashflowCategories")]
        public async Task<IActionResult> CreateUpdateCashFlowCategory(CashFlowCategoryDto cashFlowCategoryDto)
        {
            await _cashFlowCategoryService.CreateUpdateCashFlowCategoryAsync(cashFlowCategoryDto);//TODO
            return Ok(cashFlowCategoryDto);
        }

        [HttpGet("GetActiveCashFlowCategories")]
        public async Task<IActionResult> GetActiveCashFlowCategories()
        {
            IEnumerable<CashFlowCategoryDto> cashFlowCategories = await _cashFlowCategoryService.GetActiveCashFlowCategoriesAsync();
            return Ok(cashFlowCategories);
        }

        [HttpPost("GetCashFlowCategoryDetails")]
        public async Task<IActionResult> GetCashFlowCategoryDetails([FromBody] int cashFlowCategoryID)
        {
            CashFlowCategoryDto cashFlowCategoryDetails = await _cashFlowCategoryService.GetCashFlowCategoryDetailsAsync(cashFlowCategoryID);
            return Ok(cashFlowCategoryDetails);
        }

        [HttpPost("DeleteCashFlowCategory")]
        [Authorize("CanManageCashflowCategories")]
        public async Task<ActionResult> DeleteCashFlowCategory([FromBody] int cashFlowCategoryID)
        {
            await _cashFlowCategoryService.DeleteCashFlowCategoryAsync(cashFlowCategoryID);
            return Ok(new { Responce = "CashFlow Category Deleted Successfully." });
        }

        /// <summary>
        /// Fiscal Period
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("CreateUpdateFiscalPeriod")]
        [Authorize("CanManageFiscalPeriods")]
        public async Task<IActionResult> CreateUpdateFiscalPeriod(FiscalPeriodDto fiscalPeriodDto)
        {
            FiscalPeriodDto fiscalPeriod = await _fiscalPeriodService.CreateUpdateFiscalPeriodAsync(fiscalPeriodDto);
            return Ok(fiscalPeriodDto);
        }

        [HttpGet("GetAllFiscalPeriods")]
        public async Task<IActionResult> GetAllFiscalPeriods()
        {
            IEnumerable<FiscalPeriodDto> fiscalPeriods = await _fiscalPeriodService.GetAllFiscalPeriodsAsync();
            return Ok(fiscalPeriods);
        }

        [HttpGet("GetAllActiveFiscalPeriods")]
        public async Task<IActionResult> GetAllActiveFiscalPeriods()
        {
            IEnumerable<FiscalPeriodDto> activeFiscalPeriods = await _fiscalPeriodService.GetAllActiveFiscalPeriodsAsync();
            return Ok(activeFiscalPeriods);
        }

        [HttpGet("GetAllInactiveFiscalPeriods")]
        public async Task<IActionResult> GetAllInactiveFiscalPeriods()
        {
            IEnumerable<FiscalPeriodDto> inActiveFiscalPeriods = await _fiscalPeriodService.GetAllInActiveFiscalPeriodsAsync();
            return Ok(inActiveFiscalPeriods);
        }

        [HttpPost("GetFiscalPeriodDetails")]
        public async Task<IActionResult> GetFiscalPeriodDetails([FromBody] int fiscalPeriodID)
        {
            FiscalPeriodDto fiscalPeriodDetails = await _fiscalPeriodService.GetFiscalPeriodDetailsAsync(fiscalPeriodID);
            return Ok(fiscalPeriodDetails);
        }

        [HttpPost("CloseFiscalPeriod")]
        [Authorize("CanManageFiscalPeriods")]
        public async Task<IActionResult> CloseFiscalPeriod([FromBody] int fiscalPeriodID)
        {
            FiscalPeriodDto fiscalPeriodDto = await _fiscalPeriodService.CloseFiscalPeriodAsync(fiscalPeriodID);
            return Ok(fiscalPeriodDto);
        }

        [HttpPost("DeleteFiscalPeriod")]
        [Authorize("CanManageFiscalPeriods")]
        public async Task<IActionResult> DeleteFiscalPeriod([FromBody] int fiscalPeriodID)
        {
            await _fiscalPeriodService.DeleteFiscalPeriodAsync(fiscalPeriodID);
            return Ok(new { Responce = "Fiscal Period Deleted Successfully." });
        }

        /// <summary>
        /// Taxes
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>

        //Vat Tax
        [HttpPost("CreateUpdateVATType")]
        [Authorize("CanManageTaxes")]
        public async Task<IActionResult> CreateUpdateVATType(VatTypeDto vatTypeDto)
        {
            VatTypeDto vatType = await _taxService.CreateUpdateVATTypeAsync(vatTypeDto);
            return Ok(vatType);
        }
        [HttpGet("GetAllVATTypes")]
        public async Task<IActionResult> GetAllVATTypes()
        {
            IEnumerable<VatTypeDto> vatTypes = await _taxService.GetAllVATTypesAsync();
            return Ok(vatTypes);
        }
        [HttpPost("GetVATTypeDetails")]
        public async Task<IActionResult> GetVATTypeDetails([FromBody] int vatTypeID)
        {
            VatTypeDto vatType = await _taxService.GetVATTypeDetails(vatTypeID);
            return Ok(vatType);
        }
        [HttpPost("DeleteVATType")]
        [Authorize("CanManageTaxes")]
        public async Task<IActionResult> DeleteVATType([FromBody] int vatTypeID)
        {
            await _taxService.DeleteVATTypeAsync(vatTypeID);
            return Ok(new { Responce = "Vat Type Deleted Successfully." });
        }

        //Other Taxes
        [HttpPost("CreateUpdateOtherTax")]
        [Authorize("CanManageTaxes")]
        public async Task<IActionResult> CreateUpdateOtherTax(OtherTaxDto otherTaxDto)
        {
            OtherTaxDto otherTax = await _taxService.CreateUpdateOtherTaxAsync(otherTaxDto);
            return Ok(otherTax);
        }
        [HttpGet("GetAllOtherTaxes")]
        public async Task<IActionResult> GetAllOtherTaxes()
        {
            IEnumerable<OtherTaxDto> otherTaxes = await _taxService.GetAllOtherTaxesAsync();
            return Ok(otherTaxes);
        }
        [HttpPost("GetOtherTaxDetails")]
        public async Task<IActionResult> GetOtherTaxDetails([FromBody] int otherTaxID)
        {
            OtherTaxDto otherTaxDetails = await _taxService.GetOtherTaxDetailsAsync(otherTaxID);
            return Ok(otherTaxDetails);
        }
        [HttpPost("DeleteOtherTax")]
        [Authorize("CanManageTaxes")]
        public async Task<IActionResult> DeleteOtherTax([FromBody] int otherTaxID)
        {
            await _taxService.DeleteOtherTaxAsync(otherTaxID);
            return Ok(new { Responce = "Other Tax Deleted Successfully." });
        }

        //config
        [HttpGet("GetAllLiabilitySubAccounts")]
        public async Task<IActionResult> GetAllLiabilitySubAccounts()
        {
            IEnumerable<SubAccountDto> subAccounts = await _taxService.GetAllLiabilitySubAccountsAsync();
            return Ok(subAccounts);
        }
    }
}