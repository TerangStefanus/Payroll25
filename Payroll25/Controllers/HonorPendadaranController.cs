using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;

namespace Payroll25.Controllers
{
    [Authorize]
    public class HonorPendadaranController : Controller
    {
        private readonly HonorPendadaranDAO DAO;

        public HonorPendadaranController()
        {
            DAO = new HonorPendadaranDAO();
        }

        public async Task<IActionResult> Index(string NPPFilter = null, string NAMAFilter = null)
        {
            try
            {
                var honoPendadaranList = await DAO.ShowHonorPendadaranAsync(NPPFilter, NAMAFilter) ?? new List<HonorPendadaranModel>();
                var komponenGajiList = await DAO.GetKomponenGaji();

                ViewBag.KomponenGajiList = komponenGajiList; // Set data to ViewBag

                var viewModel = new HonorPendadaranModel.HonorPendadaranViewModel
                {
                    HonorPendadaranList = honoPendadaranList,
                    NPPFilter = NPPFilter,
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
        public IActionResult InsertHonorPendadaran([FromBody] HonorPendadaranModel model)
        {
            DBOutput data = new DBOutput();
            var success = DAO.InsertHonorPendadaran(model);

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

        [HttpPost]
        public IActionResult UpdateHonorPendadaran([FromBody] List<HonorPendadaranModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.UpdateHonorPendadaran(model);
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
