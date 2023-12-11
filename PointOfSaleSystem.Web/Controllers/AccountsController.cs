using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PointOfSaleSystem.Web.Controllers
{
    [Route("Accounts")]
    [Authorize]
    public class AccountsController : Controller
    {
        [Route("GeneralLedgerAccounts")]
        [Route("/")]
        public IActionResult GeneralLedgerAccounts()
        {
            return View();
        }

        [Route("FiscalPeriods")]
        public IActionResult FiscalPeriods()
        {
            return View();
        }

        [Route("ActivateSubAccount")]
        public IActionResult ActivateSubAccount()
        {
            return View();
        }
        [Route("DeactivateSubAccount")]
        public IActionResult DeactivateSubAccount()
        {
            return View();
        }

        [Route("JournalVouchers")]
        public IActionResult JournalVouchers()
        {
            return View();
        }
        [Route("GetAllAccountsAsync")]
        public IActionResult GetAllAccountsAsync()
        {
            return View();
        }
        [Route("Taxes")]
        public IActionResult Taxes()
        {
            return View();
        }
    }
}
