using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System;
using System.Threading.Tasks;
using static Payroll25.Models.TunjanganPengabdianModel;

namespace Payroll25.Controllers
{
    public class TunjanganPengabdianController : Controller
    {
        private readonly TunjanganPengabdianDAO DAO;

        public TunjanganPengabdianController()
        {
            DAO = new TunjanganPengabdianDAO();
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = new TunjanganViewModel
                {
                    TunjanganPengabdianList = await DAO.ShowTunjanganPengabdianAsync(),
                    TunjanganPengabdian = new TunjanganPengabdianModel()
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                // Handle exceptions here
                return StatusCode(500);
            }
        }

        // GET: TunjanganPengabdianController/Details/5
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
                return BadRequest("Failed to get Komponen Gaji data.");
            }
        }

        // POST: TunjanganPengabdian/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TunjanganViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new TunjanganViewModel();
            }
            else if (viewModel != null)
            {
                var errors = new List<string>();

                if (viewModel.TunjanganPengabdian.ID_KOMPONEN_GAJI == 0)
                {
                    errors.Add("ID Komponen Gaji harus diisi.");

                }

                if (viewModel.TunjanganPengabdian.ID_BULAN_GAJI == 0)
                {
                    errors.Add("ID BULAN GAJI harus diisi.");

                }

                if (string.IsNullOrEmpty(viewModel.TunjanganPengabdian.NPP))
                {
                    errors.Add("NPP harus diisi.");

                }

                if (viewModel.TunjanganPengabdian.JUMLAH == 0)
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
            viewModel.TunjanganPengabdianList = await DAO.ShowTunjanganPengabdianAsync();
            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDetails(TunjanganViewModel viewModel, int ID_Vakasi)
        {
            try
            {
                bool updateResult = DAO.UpdateVakasiTunjangan(viewModel,ID_Vakasi);

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
    }
}
