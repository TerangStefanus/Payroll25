using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;

namespace Payroll25.Controllers
{
    public class PenggajianController : Controller
    {
        private readonly PenggajianDAO DAO;

        public PenggajianController()
        {
            DAO = new PenggajianDAO();
        }

        public async Task<IActionResult> Index(string NPPFilter = null, string NAMAFilter = null)
        {
            try
            {
                var penggajianList = await DAO.GetPenggajianDataAsync(NPPFilter) ?? new List<PenggajianModel>();

                if (!string.IsNullOrEmpty(NAMAFilter))
                {
                    penggajianList = penggajianList.Where(p => p.NAMA.Contains(NAMAFilter)).ToList();
                }

                var viewModel = new PenggajianModel.PenggajianViewModel
                {
                    PenggajianList = penggajianList,
                    NPPFilter = NPPFilter,
                    NAMAFilter = NAMAFilter
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan saat mengambil data: {ex.Message}");
            }
        }





    }
}
