using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System;
using System.Threading.Tasks;
using static Payroll25.Models.HonorUjianModel;


namespace Payroll25.Controllers
{
    public class HonorUjianController : Controller
    {
        private readonly HonorUjianDAO DAO;

        public HonorUjianController()
        {
            DAO = new HonorUjianDAO();
        }

        public async Task<IActionResult> Index(string NPPFilter = null, string NAMAFilter = null)
        {
            try
            {
                var honorUjianList = await DAO.ShowHonorUjianAsync(NPPFilter, NAMAFilter) ?? new List<HonorUjianModel>();
                var komponenGajiList = await DAO.GetKomponenGaji();

                ViewBag.KomponenGajiList = komponenGajiList; // Set data to ViewBag

                var viewModel = new HonorUjianModel.HonorUjianViewModel 
                {
                    HonorUjianList = honorUjianList,
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
        public IActionResult InsertHonorUjian([FromBody] HonorUjianModel model)
        {
            DBOutput data = new DBOutput();
            var success = DAO.InsertHonorUjian(model);

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
        public IActionResult UpdateHonorUjian([FromBody] List<HonorUjianModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.UpdateHonorUjian(model);
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
        public IActionResult DeleteHonorUjian([FromBody] List<HonorUjianModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.DeleteHonorUjian(model);
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
