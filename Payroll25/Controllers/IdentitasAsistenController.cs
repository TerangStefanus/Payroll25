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


namespace Payroll25.Controllers
{
    public class IdentitasAsistenController : Controller
    {
        private readonly IdentitasAsistenDAO DAO;

        public IdentitasAsistenController()
        {
            DAO = new IdentitasAsistenDAO();
        }

        // GET: IdentitasAsisten
        public async Task<IActionResult> Index(string NPMFilter = null, string NAMAFilter = null, string UNITFilter = null)
        {
            try
            {
                var identitasAsistenList = await DAO.ShowIdentitasAssisten(NPMFilter, NAMAFilter, UNITFilter) ?? new List<IdentitasAsistenModel>();

                var viewModel = new IndexViewModel
                {
                    IdentitasAsistenList = identitasAsistenList,
                    IdentitasAsisten = new IdentitasAsistenModel(),
                    UnitsList = (List<IdentitasAsistenModel>)DAO.GetUnit(),
                    JenisAsistenList = (List<IdentitasAsistenModel>)DAO.GetJenisAsisten(),
                    NPMFilter = NPMFilter,
                    NAMAFilter = NAMAFilter,
                    UNITFilter = UNITFilter
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan saat mengambil data : {ex.Message}");
            }

        }


        // GET: IdentitasAsistenController/Details/5
        [HttpGet]
        public IActionResult GetDetails(string npm)
        {
            IdentitasAsistenModel detailAsisten = DAO.GetDetails(npm);

            if (detailAsisten != null)
            {
                IndexViewModel viewModel = new IndexViewModel
                {
                    IdentitasAsisten = detailAsisten
                };
                TempData["success"] = "Berhasil mengubah data!";
            }
            else
            {
                TempData["error"] = "Gagal Mengambil Data";
            }

            return RedirectToAction("IdentitasAsisten");
        }


        // POST: IdentitasAsisten/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IndexViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new IndexViewModel();
            }
            else if (viewModel != null)
            {
                var errors = new List<string>();

                if (viewModel.IdentitasAsisten.ID_TAHUN_AKADEMIK == 0)
                {
                    errors.Add("ID Tahun Akademik harus diisi.");
                }

                if (viewModel.IdentitasAsisten.NO_SEMESTER == 0)
                {
                    errors.Add("No Semester harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasAsisten.NPM))
                {
                    errors.Add("NPM harus diisi.");
                }

                if (viewModel.IdentitasAsisten.ID_UNIT == 0)
                {
                    errors.Add("ID Unit harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasAsisten.NO_REKENING))
                {
                    errors.Add("NO_REKENING harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasAsisten.NAMA_REKENING))
                {
                    errors.Add("NAMA_REKENING harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasAsisten.NAMA_BANK))
                {
                    errors.Add("NAMA_BANK harus diisi.");
                }

                if (viewModel.IdentitasAsisten.ID_JENIS_ASISTEN == 0)
                {
                    errors.Add("ID Jenis Asisten harus diisi.");
                }


                if (errors.Count == 0)
                {
                    // Melakukan insert Identitas Asisten ke database menggunakan objek viewModel
                    bool insertResult = DAO.InsertDetails(viewModel);

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

            //viewModel.IdentitasAsistenList = DAO.ShowIdentitasAssisten();
            return View("Index", viewModel);
        }



        // GET: IdentitasAsistenController/Edit/5
        public IActionResult Edit(int id)
        {
            return View();
        }

        // POST: IdentitasAsistenController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDetails(IndexViewModel viewModel, int ID_Asisten)
        {
            try
            {
                bool updateResult = DAO.UpdateDetails(viewModel, ID_Asisten);

                if (updateResult)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Terjadi kesalahan saat menyimpan ke database.");
                }
            }

            catch (Exception )
            {
                // Handle the exception
                ModelState.AddModelError("", "Terjadi Error saat update details. Tolong Coba Lagi."); // Menambahkan pesan error ke ModelState
            }

            // If the execution reaches this point, there was an error, so return the view with the updated ModelState
            return View("Index", viewModel);
        } 

        // GET: IdentitasAsistenController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: IdentitasAsistenController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(IndexViewModel viewModel, int ID_Asisten)
        {
            try
            {
                bool updateResult = DAO.DeleteDetails(viewModel, ID_Asisten);

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
                var config = new CsvConfiguration(CultureInfo.InvariantCulture){};
 
                using var memoryStream = new MemoryStream();
                using var streamWriter = new StreamWriter(memoryStream);
                using var csvWriter = new CsvWriter(streamWriter, config);
                csvWriter.Context.RegisterClassMap<IdentitasAsistenModelMap>();

                var templateRecord = new IdentitasAsistenModel
                {
                    ID_TAHUN_AKADEMIK = 0,
                    NO_SEMESTER =0,
                    NPM = "Isi NPM disini", 
                    NAMA_MHS = "Isi NAMA disini",
                    ID_UNIT = 0,
                    NO_REKENING = "Isi NOMER disini",
                    NAMA_REKENING = "Isi REKENING disini",
                    NAMA_BANK = "Isi BANK disini",
                    ID_JENIS_ASISTEN = 0,
                };

                var templateList = new List<IdentitasAsistenModel> { templateRecord };

                csvWriter.WriteRecords(templateList);

                streamWriter.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);

                return File(memoryStream.ToArray(), "text/csv", "IdentitasAsisten_Template.csv");
            }
            catch (Exception ex)
            {
                return BadRequest("Terjadi kesalahan saat mencoba mendownload CSV: " + ex.Message);
            }
        }














    }
}
