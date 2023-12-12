using Microsoft.AspNetCore.Mvc;
using System.Data;
using FastReport.Data;
using FastReport.Utils;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;

namespace PointOfSaleSystem.Web.Controllers
{
    [Route("Dashboard")]
    [Authorize]
    public class DashboardController : Controller
    {
        [Route("Accounts")]
        public IActionResult Accounts()
        {
            return View();
        }

        [Route("Inventory")]
        public IActionResult Inventory()
        {
            return View();
        }
    }
}
