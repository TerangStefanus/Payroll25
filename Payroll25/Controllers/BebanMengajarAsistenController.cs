using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;

namespace Payroll25.Controllers
{
    [Authorize]
    public class BebanMengajarAsistenController : Controller
    {
        private readonly BebanMengajarAsistenDAO DAO;

        public BebanMengajarAsistenController()
        {
            DAO = new BebanMengajarAsistenDAO();
        }

        public async Task<IActionResult> Index(string NPMFilter = null, string NAMAFilter = null)
        {
            try
            {
                var bebanMengajarAsistenList = await DAO.ShowBebanMengajarAsistenAsync(NPMFilter, NAMAFilter) ?? new List<BebanMengajarAsistenModel>();

                var viewModel = new BebanMengajarAsistenModel.BebanMengajarAsistenViewModel
                {
                    BebanMengajarAsistenList = bebanMengajarAsistenList,
                    NPMFilter = NPMFilter,
                    NAMAFilter = NAMAFilter
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan saat mengambil data : {ex.Message}");
            }

        }

        [HttpPost]
        public IActionResult UpdateBebanMengajarAsisten([FromBody] List<BebanMengajarAsistenModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.UpdateBebanMengajarAsisten(model);
            if (success != 0)
            {
                data.status = true;
                data.pesan = " Update berhasil ";
            }
            else
            {
                data.status = false;
                data.pesan = " Update gagal";
            }

            return Json(data);
        }

        [HttpPost]
        public IActionResult InsertBebanMengajarAsisten([FromBody] BebanMengajarAsistenModel model)
        {
            DBOutput data = new DBOutput();
            var success = DAO.InsertBebanMengajarAsisten(model);

            if (success != 0)
            {
                data.status = true;
                data.pesan = "Insert berhasil!";
            }
            else
            {
                data.status = false;
                data.pesan = "Insert gagal!";
            }

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetBulanGajiDropdown(int tahun)
        {
            try
            {
                var bulanGajiList = await DAO.GetBulanGaji(tahun);

                // Pastikan model yang dikembalikan sesuai dengan model yang digunakan di view
                var result = bulanGajiList.Select(k => new { id_bulan = k.GET_BULAN_GAJI });

                return Json(result);
            }
            catch (Exception)
            {
                // Handle exceptions here
                return BadRequest("Failed to get Bulan Gaji data.");
            }
        }





    }
}
