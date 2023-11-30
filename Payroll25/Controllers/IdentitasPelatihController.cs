using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using static Payroll25.Models.IdentitasPelatihModel;
using ClosedXML.Excel;

namespace Payroll25.Controllers
{
    public class IdentitasPelatihController : Controller
    {
        IdentitasPelatihDAO DAO;

        public IdentitasPelatihController()
        {
            DAO = new IdentitasPelatihDAO();
        }

        // GET: IdentitasPelatih
        public IActionResult Index()
        {
            var viewModel = new IdentitasPelatihViewModel
            {
                IdentitasPelatihList = DAO.ShowIdentitasPelatih(),
                IdentitasPelatih = new IdentitasPelatihModel(),
                UnitsList = (List<IdentitasPelatihModel>)DAO.GetUnit(),
            };

            return View(viewModel);
        }

        // POST: IdentitasAsisten/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IdentitasPelatihViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new IdentitasPelatihViewModel();
            }
            else if (viewModel != null)
            {
                var errors = new List<string>();

                if (string.IsNullOrEmpty(viewModel.IdentitasPelatih.NPP))
                {
                    errors.Add("NPP harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasPelatih.NAMA))
                {
                    errors.Add("NAMA harus diisi.");
                }

                if (viewModel.IdentitasPelatih.ID_TAHUN_AKADEMIK == 0)
                {
                    errors.Add("ID Tahun Akademik harus diisi.");
                }

                if (viewModel.IdentitasPelatih.NO_SEMESTER == 0)
                {
                    errors.Add("No Semester harus diisi.");
                }

                if (viewModel.IdentitasPelatih.ID_UNIT == 0)
                {
                    errors.Add("ID Unit harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasPelatih.NO_REKENING))
                {
                    errors.Add("NO_REKENING harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasPelatih.NAMA_REKENING))
                {
                    errors.Add("NAMA_REKENING harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasPelatih.NAMA_BANK))
                {
                    errors.Add("NAMA_BANK harus diisi.");
                }


                if (errors.Count == 0)
                {
                    // Melakukan insert Identitas Asisten ke database menggunakan objek viewModel
                    bool insertResult = DAO.InsertIdentitasPelatih(viewModel);

                    if (insertResult)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Terjadi kesalahan saat menyimpan ke database.");
                    }
                }
                else
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
            }
            else
            {
                return BadRequest("Data tidak valid. Mohon isi formulir dengan benar.");
            }

            viewModel.IdentitasPelatihList = DAO.ShowIdentitasPelatih();
            return View("Index", viewModel);
        }

        public IActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDetails(IdentitasPelatihViewModel viewModel, int ID_Pelatih)
        {
            try
            {
                bool updateResult = DAO.UpdateIdentitasPelatih(viewModel, ID_Pelatih);

                if (updateResult)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Terjadi kesalahan saat menyimpan ke database.");
                }
            }

            catch (Exception)
            {
                // Handle the exception
                ModelState.AddModelError("", "Terjadi Error saat update details. Tolong Coba Lagi."); // Menambahkan pesan error ke ModelState
            }

            // If the execution reaches this point, there was an error, so return the view with the updated ModelState
            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(IdentitasPelatihViewModel viewModel, int ID_Pelatih)
        {
            try
            {
                bool updateResult = DAO.DeleteIdentitasPelatih(viewModel, ID_Pelatih);

                if (updateResult)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Terjadi kesalahan saat menghapus Data.");
                }
            }

            catch (Exception)
            {
                // Handle Error
                ModelState.AddModelError("", "Terjadi Error saat update details. Tolong Coba Lagi."); // Menambahkan pesan error ke ModelState
            }

            // Ketika Data di eksekusi pada point ini maka terjadi error 
            return View("Index", viewModel);
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

        public IActionResult DownloadExcelTemplate()
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("TemplateSheet");

                    // Adding headers
                    worksheet.Cell(1, 1).Value = "NPP";
                    worksheet.Cell(1, 2).Value = "NAMA";
                    worksheet.Cell(1, 3).Value = "ID_TAHUN_AKADEMIK";
                    worksheet.Cell(1, 4).Value = "NO_SEMESTER";
                    worksheet.Cell(1, 5).Value = "ID_UNIT";
                    worksheet.Cell(1, 6).Value = "NO_REKENING";
                    worksheet.Cell(1, 7).Value = "NAMA_REKENING";
                    worksheet.Cell(1, 8).Value = "NAMA_BANK";

                    // Adding example data
                    worksheet.Cell(2, 1).Value = "Isi NPP disini";
                    worksheet.Cell(2, 2).Value = "Isi NAMA disini";
                    worksheet.Cell(2, 3).Value = 0;
                    worksheet.Cell(2, 4).Value = 0;
                    worksheet.Cell(2, 5).Value = 0;
                    worksheet.Cell(2, 6).Value = "Isi NOMER disini";
                    worksheet.Cell(2, 7).Value = "Isi REKENING disini";
                    worksheet.Cell(2, 8).Value = "Isi BANK disini";

                    workbook.SaveAs(memoryStream);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "IdentitasPelatih_Template.xlsx");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Terjadi kesalahan saat mencoba mendownload Excel: " + ex.Message);
            }
        }


    }
}
