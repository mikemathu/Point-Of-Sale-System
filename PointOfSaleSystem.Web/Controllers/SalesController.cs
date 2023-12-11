using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PointOfSaleSystem.Web.Controllers
{
    [Route("Sales")]
    [Authorize]
    public class SalesController : Controller
    {
        [Route("PointOfSale")]
        public IActionResult PointOfSale()
        {
            return View();
        }
    }
}
