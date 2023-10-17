using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;

namespace Payroll25.Controllers
{
    public class TunjanganPengabdianController : Controller
    {
        private readonly TunjanganPengabdianDAO DAO;

        public TunjanganPengabdianController()
        {
            DAO = new TunjanganPengabdianDAO();
        }

        public async Task<IActionResult> Index(string NPPFilter = null, string NAMAFilter = null,string NPMFilter = null)
        {
            try
            {
                var tunjanganPengabdianList = await DAO.ShowTunjanganPengabdianAsync(NPPFilter, NAMAFilter,NPMFilter) ?? new List<TunjanganPengabdianModel>();
                var komponenGajiList = await DAO.GetKomponenGaji();

                ViewBag.KomponenGajiList = komponenGajiList; // Set data to ViewBag

                var viewModel = new TunjanganPengabdianModel.TunjanganPengabdianViewModel
                {
                    TunjanganPengabdianList = tunjanganPengabdianList,
                    NPPFilter = NPPFilter,
                    NAMAFilter = NAMAFilter,
                    NPMFilter = NPMFilter
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan saat mengambil data : {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult InsertTunjanganPengabdian([FromBody] TunjanganPengabdianModel model)
        {
            DBOutput data = new DBOutput();
            var success = DAO.InsertTunjanganPengabdian(model);

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
        public IActionResult UpdateTunjanganPengabdian([FromBody] List<TunjanganPengabdianModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.UpdateTunjanganPengabdian(model);
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
        public IActionResult DeleteTunjanganPengabdian([FromBody] List<TunjanganPengabdianModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.DeleteTunjanganPengabdian(model);
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
