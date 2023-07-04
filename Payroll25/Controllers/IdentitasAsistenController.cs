using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Dynamic;
using System.Reflection;

namespace Payroll25.Controllers
{
    public class IdentitasAsistenController : Controller
    {
        IdentitasAsistenDAO DAO;

        public IdentitasAsistenController()
        {
            DAO = new IdentitasAsistenDAO();
        }

        // GET: IdentitasAsisten
        public IActionResult Index()
        {
            var viewModel = new IndexViewModel
            {
                IdentitasAsistenList = DAO.ShowIdentitasAssisten(),
                IdentitasAsisten = new IdentitasAsistenModel()
            };

            return View(viewModel);
        }


        // GET: IdentitasAsistenController/Details/5
        public ActionResult GetDetails(string npm)
        {
            // Panggil metode di DAO untuk mendapatkan detail asisten berdasarkan NPM
            IdentitasAsistenModel detailAsisten = DAO.GetDetails(npm);

            if (detailAsisten != null)
            {
                return View(detailAsisten);
            }
            else
            {
                return Content("Detail asisten tidak ditemukan.");
            }
        }

        // POST: IdentitasAsisten/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IndexViewModel viewModel)
        {
            if (viewModel != null)
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

            viewModel.IdentitasAsistenList = DAO.ShowIdentitasAssisten();
            return View("Index", viewModel);
        }



        // GET: IdentitasAsistenController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: IdentitasAsistenController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: IdentitasAsistenController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: IdentitasAsistenController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
