using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Globalization;
using ClosedXML.Excel;

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
        public async Task<IActionResult> UploadExcel(IFormFile excelFile)
        {
            var result = new { success = false, errorMessage = string.Empty };

            if (excelFile == null || excelFile.Length == 0)
            {
                result = new { success = false, errorMessage = "File tidak ditemukan" };
                return Json(result);
            }

            try
            {
                using (var stream = excelFile.OpenReadStream())
                using (var workbook = new XLWorkbook(stream))
                {
                    var uploadResult = DAO.UploadAndInsertExcel(excelFile);

                    if (uploadResult.Item1)
                    {
                        TempData["success"] = "Berhasil mengunggah dan memproses Excel!";
                        result = new { success = true, errorMessage = string.Empty }; // Only set success to true if upload was successful
                    }
                    else
                    {
                        TempData["error"] = "Gagal mengunggah dan memproses Excel.";
                        if (uploadResult.Item2.Any())
                        {
                            TempData["validationErrors"] = string.Join(", ", uploadResult.Item2);
                        }
                        result = new { success = false, errorMessage = "Gagal mengunggah dan memproses Excel." }; // Set success to false
                    }
                }
            }
            catch (Exception ex)
            {
                result = new { success = false, errorMessage = "Gagal memproses file Excel: " + ex.Message };
            }


            return Json(result);
        }

        public IActionResult DownloadExcel()
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("TemplateSheet");

                    // Adding headers
                    worksheet.Cell(1, 1).Value = "ID_KOMPONEN_GAJI";
                    worksheet.Cell(1, 2).Value = "ID_BULAN_GAJI";
                    worksheet.Cell(1, 3).Value = "NPP";
                    worksheet.Cell(1, 4).Value = "JUMLAH";

                    // Adding example data
                    worksheet.Cell(2, 1).Value = 0;
                    worksheet.Cell(2, 2).Value = 0;
                    worksheet.Cell(2, 3).Value = "Isi NPP disini";
                    worksheet.Cell(2, 4).Value = 0;

                    workbook.SaveAs(memoryStream);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Honor_Template.xlsx");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Terjadi kesalahan saat mencoba mendownload Excel: " + ex.Message);
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
