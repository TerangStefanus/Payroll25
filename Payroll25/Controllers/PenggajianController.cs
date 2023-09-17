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
                var kontrakPenggajianList = await DAO.GetKontrakPenggajianDataAsync(NPPFilter, NAMAFilter);

                var viewModel = new PenggajianModel.PenggajianViewModel
                {
                    PenggajianList = penggajianList,
                    KontrakPenggajianList = kontrakPenggajianList,
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

        [HttpPost]
        public IActionResult InsertKontrakPenggajianData([FromBody] PenggajianModel model)
        {
            DBOutput data = new DBOutput();
            var success = DAO.InsertKontrakPenggajianData(model);

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
        public IActionResult UpdateKontrakPenggajianData([FromBody] List<PenggajianModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.UpdateKontrakPenggajianData(model);
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
