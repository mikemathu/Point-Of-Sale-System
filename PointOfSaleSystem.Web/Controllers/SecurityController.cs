using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PointOfSaleSystem.Web.Controllers
{

    [Route("Security")]
    [Authorize]
    public class SecurityController : Controller
    {
        [Route("Register")]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [Route("Roles")]
        public IActionResult Roles()
        {
            return View();
        }

        [Route("Privileges")]
        public IActionResult Privileges()
        {
            return View();
        }
        [Route("SystemUsers")]
        public IActionResult SystemUsers()
        {
            return View();
        }
    }
}