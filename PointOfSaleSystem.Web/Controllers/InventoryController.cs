using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PointOfSaleSystem.Web.Controllers
{
    [Route("Inventory")]
    [Authorize]
    public class InventoryController : Controller
    {
        [Route("Inventory")]
        public IActionResult Inventory()
        {
            return View();
        }
        [Route("StockTake")]
        public IActionResult StockTake()
        {
            return View();
        }
        [Route("UnitOfMeasure")]
        public IActionResult UnitOfMeasure()
        {
            return View();
        }
    }
}
