using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Dynamic;
using System.Reflection;
using System.Security.Cryptography;
using static Payroll25.Models.IdentitasPelatihModel;

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






    }
}
