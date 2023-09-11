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
                var komponenGajiList = await DAO.GetKomponenGaji();

                var viewModel = new KelolaTarifModel.KelolaTarifViewModel
                {
                    KelolaTarifList = kelolaTarifList,
                    NAMAFilter = NAMAFilter
                };

                ViewBag.KomponenGajiList = komponenGajiList;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan saat mengambil data : {ex.Message}");
            }
        }


        [HttpPost]
        public IActionResult UpdateTarif([FromBody] List<KelolaTarifModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.UpdateKelolaTarif(model);
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




    }
}
