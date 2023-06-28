using Microsoft.AspNetCore.Mvc;

namespace Payroll25.Controllers
{
    public class PayslipController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Payslip_User() 
        {
            return View();
        }


    }
}
