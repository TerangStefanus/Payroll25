using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Globalization;

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
                var komponenGajiList = await DAO.GetKomponenGaji();

                ViewBag.KomponenGajiList = komponenGajiList; // Set data to ViewBag

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

        [HttpPost]
        public IActionResult DeleteKontrakPenggajianData([FromBody] List<PenggajianModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.DeleteKontrakPenggajianData(model);
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


        [HttpPost]
        public IActionResult InsertDetailPenggajian([FromBody] PenggajianModel model)
        {
            DBOutput data = new DBOutput();
            var success = DAO.InsertDetailPenggajian(model);

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
        public async Task<IActionResult> UploadCSV(IFormFile CsvFile)
        {
            var result = new { success = false, errorMessage = string.Empty };

            if (CsvFile == null || CsvFile.Length == 0)
            {
                result = new { success = false, errorMessage = "File tidak ditemukan" };
                return Json(result);
            }

            try
            {
                using (var reader = new StreamReader(CsvFile.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    var records = csv.GetRecords<PenggajianModel>().ToList();

                    var uploadResult = DAO.UploadAndInsertCSV(CsvFile);

                    if (uploadResult.Item1)
                    {
                        TempData["success"] = "Berhasil mengunggah dan memproses CSV!";
                    }
                    else
                    {
                        TempData["error"] = "Gagal mengunggah dan memproses CSV.";
                        if (uploadResult.Item2.Any())
                        {
                            TempData["validationErrors"] = string.Join(", ", uploadResult.Item2);
                        }
                    }
                }

                result = new { success = true, errorMessage = string.Empty };
            }
            catch (Exception ex)
            {
                result = new { success = false, errorMessage = "Gagal memproses file CSV: " + ex.Message };
            }

            return Json(result);
        }

        public IActionResult DownloadCSV()
        {
            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture) { };

                using var memoryStream = new MemoryStream();
                using var streamWriter = new StreamWriter(memoryStream);
                using var csvWriter = new CsvWriter(streamWriter, config);
                csvWriter.Context.RegisterClassMap<PenggajianModelMap>();

                var templateRecord = new PenggajianModel
                {
                    ID_PENGGAJIAN = 0,
                    ID_KOMPONEN_GAJI = 0,
                    JUMLAH_SATUAN = 0,
                    NOMINAL = 0
                };

                var templateList = new List<PenggajianModel> { templateRecord };

                csvWriter.WriteRecords(templateList);

                streamWriter.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);

                return File(memoryStream.ToArray(), "text/csv", "PenggajianDetail_Template.csv");
            }
            catch (Exception ex)
            {
                return BadRequest("Terjadi kesalahan saat mencoba mendownload CSV: " + ex.Message);
            }
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
