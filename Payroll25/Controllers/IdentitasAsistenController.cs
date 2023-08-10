using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Dynamic;
using System.Reflection;
using System.Security.Cryptography;

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
                IdentitasAsisten = new IdentitasAsistenModel(),
                UnitsList = (List<IdentitasAsistenModel>)DAO.GetUnit()
            };

            return View(viewModel);
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



    }
}
