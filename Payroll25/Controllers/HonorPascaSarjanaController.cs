using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;

namespace Payroll25.Controllers
{
    public class HonorPascaSarjanaController : Controller
    {
        private readonly HonorPascaSarjanaDAO DAO;

        public HonorPascaSarjanaController()
        {
            DAO = new HonorPascaSarjanaDAO();
        }

        public async Task<IActionResult> Index(string NPPFilter = null, string NAMAFilter = null)
        {
            try
            {
                var honorPascaSarjanaList = await DAO.ShowHonorPascaSarjanaAsync(NPPFilter, NAMAFilter) ?? new List<HonorPascaSarjanaModel>();
                var komponenGajiList = await DAO.GetKomponenGaji();

                ViewBag.KomponenGajiList = komponenGajiList; // Set data to ViewBag

                var viewModel = new HonorPascaSarjanaModel.HonorPascaSarjanaViewModel
                {
                    HonorPascaSarjanaList = honorPascaSarjanaList,
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
        public IActionResult InsertHonorPascaSarjana([FromBody] HonorPascaSarjanaModel model)
        {
            DBOutput data = new DBOutput();
            var success = DAO.InsertHonorPascaSarjana(model);

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
        public IActionResult UpdateHonorPascaSarjana([FromBody] List<HonorPascaSarjanaModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.UpdateHonorPascaSarjana(model);
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
        public IActionResult DeleteHonorPascaSarjana([FromBody] List<HonorPascaSarjanaModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.DeleteHonorPascaSarjana(model);
            if (success != 0)
            {
                data.status = true;
                data.pesan = " Delete data berhasil ";
            }
            else
            {
                data.status = false;
                data.pesan = " Delete data gagal";
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
