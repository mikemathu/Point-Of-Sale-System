using FastReport.Data;
using FastReport.Utils;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointOfSaleSystem.Service.Dtos.Dashboard;
using PointOfSaleSystem.Service.Services.Dashboard;
using PointOfSaleSystem.Web.Filters;
using System.Data;

namespace PointOfSaleSystem.Web.ApiControllers
{
    [Route("Dashboard")]
    [ApiController]
    [Authorize]
    [ValidateModel, HandleException]
    public class DashboardController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ReportsPaths _reportsPaths;
        public DashboardController(IWebHostEnvironment hostingEnvironment, ReportsPaths accountsReports)
        {
            _hostingEnvironment = hostingEnvironment;
            _reportsPaths = accountsReports;
        }
        [HttpPost("GetAccountsReportPath")]
        /* public IActionResult GetAccountsReportPath(DashboardReportDto dashboardReportDto)
         {
             string reportLocation = _reportsPaths.GetAccountsReportPath(dashboardReportDto);
             //Creating a connection to PostgreSQL

             RegisteredObjects.AddConnection(typeof(PostgresDataConnection));

             var report = new WebReport(); // create object
             var data = new DataSet();
             report.Report.RegisterData(data); // data binding);

             // Get the root path of the application
             //var rootPath = _hostingEnvironment.WebRootPath;

             // Construct the full path relative to the root directory
             //var reportPath = Path.Combine(rootPath, "Dashboard", "Accounts", $"{reportLocation}");
             var reportPath = $"/Dashboard/Accounts/{reportLocation}";

             // Load the report from the specified path
             //report.Report.Load(reportPath);

             //ViewBag.WebReport = report; // send object to the View
             //return View();

             //var reportUrl = Url.Content("~/") + reportPath;

             return Ok(new { ReportUrl = reportPath });
         }*/

        public IActionResult GetAccountsReportPath(DashboardReportDto dashboardReportDto)
        {
            string reportLocation = _reportsPaths.GetAccountsReportPath(dashboardReportDto);
            //Creating a connection to PostgreSQL

            RegisteredObjects.AddConnection(typeof(PostgresDataConnection));

            var report = new WebReport(); // create object
            var data = new DataSet();
            report.Report.RegisterData(data); // data binding);

            // Get the root path of the application
            var rootPath = _hostingEnvironment.WebRootPath;

            // Construct the full path relative to the root directory
            var reportPath = Path.Combine(rootPath, "Dashboard", "Accounts", $"{reportLocation}");

            // Load the report from the specified path
            report.Report.Load(reportPath);

            ViewBag.WebReport = report; // send object to the View
            return View();
        }

        [HttpPost("GetInventoryReportPath")]
        public IActionResult GetInventoryReportPath(DashboardReportDto dashboardReportDto)
        {
            string reportLocation = _reportsPaths.GetInventoryReportPath(dashboardReportDto);
            //Creating a connection to PostgreSQL

            RegisteredObjects.AddConnection(typeof(PostgresDataConnection));

            var report = new WebReport(); // create object
            var data = new DataSet();
            report.Report.RegisterData(data); // data binding);

            // Get the root path of the application
            var rootPath = _hostingEnvironment.WebRootPath;

            // Construct the full path relative to the root directory
            var reportPath = Path.Combine(rootPath, "Dashboard", "Inventory", $"{reportLocation}");

            // Load the report from the specified path
            report.Report.Load(reportPath);

            ViewBag.WebReport = report; // send object to the View
            return View();
        }
    }
}
