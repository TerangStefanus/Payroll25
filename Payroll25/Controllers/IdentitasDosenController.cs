using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using static Payroll25.Models.IdentitasDosenModel;

namespace Payroll25.Controllers
{
    
    public class IdentitasDosenController : Controller
    {
        IdentitasDosenDAO DAO;

        public IdentitasDosenController()
        {
            DAO = new IdentitasDosenDAO();
        }

        public IActionResult Index()
        {
            var viewModel = new IdentitasDosenViewModel
            {
                IdentitasDosenList = DAO.ShowIdentitasDosen(),
                IdentitasDosen = new IdentitasDosenModel(),
                NPPList = (List<IdentitasDosenModel>)DAO.GetNPP()
            };

            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IdentitasDosenViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new IdentitasDosenViewModel();
            }
            else if (viewModel != null)
            {
                var errors = new List<string>();

                if (string.IsNullOrEmpty(viewModel.IdentitasDosen.NO_REKENING))
                {
                    errors.Add("NO_REKENING harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasDosen.NPP))
                {
                    errors.Add("NPP harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasDosen.NAMA_BANK))
                {
                    errors.Add("NAMA_BANK harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasDosen.STATUS_REKENING))
                {
                    errors.Add("STATUS_REKENING harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasDosen.STATUS))
                {
                    errors.Add("STATUS harus diisi.");
                }

                if (string.IsNullOrEmpty(viewModel.IdentitasDosen.NAMA_REKENING))
                {
                    errors.Add("NAMA_REKENING harus diisi.");
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

            viewModel.IdentitasDosenList = DAO.ShowIdentitasDosen();
            return View("Index", viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDetails(IdentitasDosenViewModel viewModel, string NO_Rekening)
        {
            try
            {
                bool updateResult = DAO.UpdateDetails(viewModel, NO_Rekening);

                if (updateResult)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Terjadi kesalahan saat mengupdate data.");
                }
            }
            catch (Exception)
            {
                // Handle the exception
                ModelState.AddModelError("", "Terjadi Error saat update details. Tolong Coba Lagi."); // Menambahkan pesan error ke ModelState
            }

            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(IdentitasDosenViewModel viewModel, string NO_Rekening)
        {
            try
            {
                bool updateResult = DAO.DeleteDetails(viewModel, NO_Rekening);

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
