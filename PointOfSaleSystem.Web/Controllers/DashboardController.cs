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
        private readonly IWebHostEnvironment _hostingEnvironment;
        public DashboardController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("Accounts")]
        public IActionResult Accounts()
        {
            //Creating a connection to PostgreSQL

            RegisteredObjects.AddConnection(typeof(PostgresDataConnection));

            var report = new WebReport(); // create object
            var data = new DataSet();
            report.Report.RegisterData(data); // data binding);

            // Get the root path of the application
            var rootPath = _hostingEnvironment.WebRootPath;

            // Construct the full path relative to the root directory
            var reportPath = Path.Combine(rootPath, "Dashboard", "Invoice.frx");

            // Load the report from the specified path
            report.Report.Load(reportPath);

            ViewBag.WebReport = report; // send object to the View
            return View();
        }

        [Route("Inventory")]
        public IActionResult Inventory()
        {
            //Creating a connection to PostgreSQL

            RegisteredObjects.AddConnection(typeof(PostgresDataConnection));

            var report = new WebReport(); // create object
            var data = new DataSet();
            report.Report.RegisterData(data); // data binding

            // Get the root path of the application
            var rootPath = _hostingEnvironment.WebRootPath;

            // Construct the full path relative to the root directory
            var reportPath = Path.Combine(rootPath, "Dashboard", "Invoice.frx");

            // Load the report from the specified path
            report.Report.Load(reportPath);

            ViewBag.WebReport = report; // send object to the View
            return View();
        }
    }
}
