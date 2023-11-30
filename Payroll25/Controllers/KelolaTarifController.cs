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

        [HttpPost]
        public IActionResult InsertKelolaTarif([FromBody] KelolaTarifModel model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            try
            {
                success = DAO.InsertKelolaTarif(model);

                if (success != 0)
                {
                    data.status = true;
                    data.pesan = "Insert berhasil";
                }
                else
                {
                    data.status = false;
                    data.pesan = "Insert gagal";
                }
            }
            catch (Exception ex)
            {
                data.status = false;
                data.pesan = $"Terjadi kesalahan saat melakukan insert: {ex.Message}";
            }

            return Json(data);
        }

        [HttpPost]
        public IActionResult DeleteKelolaTarif([FromBody] List<KelolaTarifModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            try
            {
                success = DAO.DeleteKelolaTarif(model);

                if (success != 0)
                {
                    data.status = true;
                    data.pesan = "Delete data berhasil";
                }
                else
                {
                    data.status = false;
                    data.pesan = "Delete data gagal";
                }
            }
            catch (Exception ex)
            {
                // Handle exception, log, or perform additional actions as needed
                data.status = false;
                data.pesan = $"Terjadi kesalahan: {ex.Message}";
            }

            return Json(data);
        }



    }
}
