using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Globalization;

namespace Payroll25.Controllers
{
    public class HonorSisipanController : Controller
    {
        private readonly HonorSisipanDAO DAO;

        public HonorSisipanController()
        {
            DAO = new HonorSisipanDAO();
        }

        public async Task<IActionResult> Index(string NPPFilter = null)
        {
            try
            {
                var honorSisipanList = await DAO.ShowHonorSisipan(NPPFilter) ?? new List<HonorSisipanModel>();
                var komponenGajiList = await DAO.GetKomponenGaji();

                ViewBag.KomponenGajiList = komponenGajiList; // Set data to ViewBag

                var viewModel = new HonorSisipanModel.HonorSisipanViewModel
                {
                    HonorSisipanList = honorSisipanList,
                    NPPFilter = NPPFilter
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan saat mengambil data : {ex.Message}");
            }
            return View();
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
                    var records = csv.GetRecords<IdentitasAsistenModel>().ToList();

                    var uploadResult = DAO.UploadAndInsertCSV(CsvFile); // Memanggil metode ini dengan parameter yang benar

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
                csvWriter.Context.RegisterClassMap<HonorSisipanModelMap>();

                var templateRecord = new HonorSisipanModel
                {
                    ID_KOMPONEN_GAJI = 0,
                    ID_BULAN_GAJI = 0,
                    NPP = "Isi NPMP disini",
                    JUMLAH = 0
                };

                var templateList = new List<HonorSisipanModel> { templateRecord };

                csvWriter.WriteRecords(templateList);

                streamWriter.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);

                return File(memoryStream.ToArray(), "text/csv", "HonorSisipan_Template.csv");
            }
            catch (Exception ex)
            {
                return BadRequest("Terjadi kesalahan saat mencoba mendownload CSV: " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult UpdateHonorSisipan([FromBody] List<HonorSisipanModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.UpdateHonorSisipan(model);
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
        public IActionResult DeleteHonorSisipan([FromBody] List<HonorSisipanModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.DeleteHonorSisipan(model);
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
