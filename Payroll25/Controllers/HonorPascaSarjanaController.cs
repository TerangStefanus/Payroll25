using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System;
using System.Threading.Tasks;
using static Payroll25.Models.HonorPascaSarjanaModel;
using static Payroll25.Models.HonorPendadaranModel;

namespace Payroll25.Controllers
{
    public class HonorPascaSarjanaController : Controller
    {
        private readonly HonorPascaSarjanaDAO DAO;

        public HonorPascaSarjanaController()
        {
            DAO = new HonorPascaSarjanaDAO();
        }

        // GET: HonorUjianController
        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = new HonorPascaSarjanaViewModel
                {
                    HonorPascaSarjanaList = await DAO.ShowHonorPascaSarjanaAsync(),
                    HonorPascaSarjana = new HonorPascaSarjanaModel()
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                // Handle exceptions here
                return StatusCode(500);
            }
        }

        // GET: HonorPascaSarjanaController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // Metode untuk mengirimkan ID Bulan Gaji berdasarkan input Tahun ke view dalam bentuk dropdown
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

        // POST: TunjanganPengabdian/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HonorPascaSarjanaViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new HonorPascaSarjanaViewModel();
            }
            else if (viewModel != null)
            {
                var errors = new List<string>();

                if (viewModel.HonorPascaSarjana.ID_KOMPONEN_GAJI == 0)
                {
                    errors.Add("ID Komponen Gaji harus diisi.");

                }

                if (viewModel.HonorPascaSarjana.ID_BULAN_GAJI == 0)
                {
                    errors.Add("ID BULAN GAJI harus diisi.");

                }

                if (string.IsNullOrEmpty(viewModel.HonorPascaSarjana.NPP))
                {
                    errors.Add("NPP harus diisi.");

                }

                if (viewModel.HonorPascaSarjana.JUMLAH == 0)
                {
                    errors.Add("Masukan Jumlah harus diisi.");
                }

                if (errors.Count == 0)
                {
                    // Melakukan insert Identitas Asisten ke database menggunakan objek viewModel
                    bool insertResult = await Task.Run(() => DAO.InsertVakasi(viewModel));

                    if (insertResult)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "TERJADI KESALAHAN SAAT MENYIMPAN DATA PADA DATABASE.");
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

            // Gunakan await dan .Result untuk mendapatkan hasil dari metode asynchronous
            viewModel.HonorPascaSarjanaList = await DAO.ShowHonorPascaSarjanaAsync();
            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDetails(HonorPascaSarjanaViewModel viewModel, int ID_Vakasi)
        {
            try
            {
                bool updateResult = DAO.UpdateVakasiHonor(viewModel, ID_Vakasi);

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
        public ActionResult Delete(HonorPascaSarjanaViewModel viewModel, int ID_Vakasi)
        {
            try
            {
                bool updateResult = DAO.DeleteVakasiHonor(viewModel, ID_Vakasi);

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


    }
}
