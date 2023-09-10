using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;

namespace Payroll25.Controllers
{
    public class KelolaTarifController : Controller
    {
        private readonly KelolaTarifDAO DAO;

        public KelolaTarifController()
        {
            DAO = new KelolaTarifDAO();
        }

        public async Task<IActionResult> Index(string NAMAFilter = null)
        {
            try
            {
                var kelolaTarifList = await DAO.ShowKelolaTarifAsync(NAMAFilter) ?? new List<KelolaTarifModel>();

                var viewModel = new KelolaTarifModel.KelolaTarifViewModel
                {
                    KelolaTarifList = kelolaTarifList,
                    NAMAFilter = NAMAFilter
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan saat mengambil data : {ex.Message}");
            }

        }





    }
}
