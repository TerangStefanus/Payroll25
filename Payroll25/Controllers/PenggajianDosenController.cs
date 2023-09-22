using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;

namespace Payroll25.Controllers
{
    public class PenggajianDosenController : Controller
    {
        private readonly PenggajianDosenDAO DAO;

        public PenggajianDosenController()
        {
            DAO = new PenggajianDosenDAO();

        }

        public IActionResult Index()
        {
            return View(); // Ini akan memuat view dengan nama "Index" di folder Views/PenggajianDosen
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

        [HttpGet("GetKaryawanData")]
        public async Task<IActionResult> GetKaryawanData()
        {
            var result = await DAO.GetKaryawanData();
            return Ok(result);
        }

        [HttpGet("IsDataExist/{npp}/{idBulanGaji}")]
        public async Task<IActionResult> IsDataExist(string npp, int idBulanGaji)
        {
            var result = await DAO.IsDataExist(npp, idBulanGaji);
            return Ok(result);
        }

        [HttpPost("InsertToTblPenggajian")]
        public async Task<IActionResult> InsertToTblPenggajian([FromBody] PenggajianDosenModel insertData)
        {
            var result = await DAO.InsertToTblPenggajian(insertData);
            return Ok(new { success = result });
        }

        [HttpPost]
        public async Task<IActionResult> AutoInsertPenggajian(int idBulanGaji, string tahun)
        {
            var result = await DAO.AutoInsertPenggajian(idBulanGaji, tahun);
            return Ok(new { success = result });
        }

        [HttpPost]
        public async Task<IActionResult> AutoHitungGaji(int idBulanGaji, string tahun)
        {
            try
            {
                bool isSuccess = await DAO.AutoHitungGaji(idBulanGaji, tahun);

                if (isSuccess)
                {
                    return Ok(new { success = true, message = "Penggajian berhasil dihitung." });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Penggajian gagal dihitung. Data mungkin kosong atau terjadi kesalahan lain." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, new { message = "Terjadi kesalahan internal server.", error = ex.Message });
            }
        }

    }
}
