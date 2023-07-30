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

        public async Task<IActionResult> Index(string prodi = null, string namaMK = null, string fakultas = null)
        {
            try
            {
                var viewModel = new TunjanganViewModel
                {
                    TunjanganPengabdianList = await DAO.ShowTunjanganPengabdianAsync(prodi, namaMK, fakultas),
                    ProdiFilter = prodi,
                    NamaMKFilter = namaMK,
                    FakultasFilter = fakultas
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

        // GET: TunjanganPengabdian/Create
        public async Task<IActionResult> Create(string npp)
        {
            var viewModel = new TunjanganViewModel();

            // Mendapatkan data Komponen Gaji berdasarkan NPP dan mengisi dropdown
            viewModel.TunjanganPengabdianList = await DAO.GetKomponenGaji(npp);

            return View(viewModel);
        }

        // Metode untuk mengirimkan ID Komponen Gaji berdasarkan NPP ke view dalam bentuk dropdown
        [HttpGet]
        public async Task<IActionResult> GetKomponenGajiDropdown(string npp)
        {
            try
            {
                var komponenGajiList = await DAO.GetKomponenGaji(npp);

                // Pastikan model yang dikembalikan sesuai dengan model yang digunakan di view
                var result = komponenGajiList.Select(k => new { id = k.GET_KOMPONEN_GAJI });

                return Json(result);
            }
            catch (Exception)
            {
                // Handle exceptions here
                return BadRequest("Failed to get Komponen Gaji data.");
            }
        }

        // Metode untuk mengirimkan ID Bulan Gaji berdasarkan NPP ke view dalam bentuk dropdown
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
    }
}
