using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System;
using System.Threading.Tasks;
using static Payroll25.Models.BebanMengajarModel;

namespace Payroll25.Controllers
{
    public class BebanMengajarController : Controller
    {
        private readonly BebanMengajarDAO DAO;

        public BebanMengajarController()
        {
            DAO = new BebanMengajarDAO();
        }

        public async Task<IActionResult> Index(string prodi = null, string namaMK = null, string fakultas = null)
        {
            try
            {
                var viewModel = new BebanViewModel();

                // Jika terdapat input prodi & namaMK & fakultas lakukan tampilkan model ini 
                if (!string.IsNullOrEmpty(prodi) && !string.IsNullOrEmpty(namaMK) && !string.IsNullOrEmpty(fakultas))
                {
                    viewModel.BebanMengajarList = await DAO.ShowBebanMengajarAsync(prodi, namaMK, fakultas);
                }
                // Jika terdapat input prodi & namaMK lakukan tampilkan model ini 
                else if (!string.IsNullOrEmpty(prodi) && !string.IsNullOrEmpty(namaMK))
                {
                    viewModel.BebanMengajarList = await DAO.ShowBebanMengajarAsync(prodi, namaMK);
                }
                // Jika terdapat input prodi & fakultas lakukan tampilkan model ini 
                else if (!string.IsNullOrEmpty(prodi) && !string.IsNullOrEmpty(fakultas))
                {
                    viewModel.BebanMengajarList = await DAO.ShowBebanMengajarAsync(prodi, fakultas);
                }
                // Jika terdapat input namaMK & fakultas lakukan tampilkan model ini 
                else if (!string.IsNullOrEmpty(namaMK) && !string.IsNullOrEmpty(fakultas))
                {
                    viewModel.BebanMengajarList = await DAO.ShowBebanMengajarAsync(namaMK, fakultas);
                }
                // Jika terdapat input prodi lakukan tampilkan model ini 
                else if (!string.IsNullOrEmpty(prodi))
                {
                    viewModel.BebanMengajarList = await DAO.ShowBebanMengajarAsync(prodi);
                }
                // Jika terdapat input namaMK lakukan tampilkan model ini 
                else if (!string.IsNullOrEmpty(namaMK))
                {
                    viewModel.BebanMengajarList = await DAO.ShowBebanMengajarAsync(null, namaMK);
                }
                // Jika terdapat input namaMK lakukan tampilkan model ini 
                else if (!string.IsNullOrEmpty(fakultas))
                {
                    viewModel.BebanMengajarList = await DAO.ShowBebanMengajarAsync(null, null, fakultas);
                }
                // Jika tidak terdapat input tampilkan model ini
                else
                {
                    viewModel.BebanMengajarList = await DAO.ShowBebanMengajarAsync();
                }

                viewModel.ProdiFilter = prodi;
                viewModel.NamaMKFilter = namaMK;
                viewModel.FakultasFilter = fakultas;

                return View(viewModel);
            }
            catch (Exception)
            {
                // Handle exceptions here
                return StatusCode(500);
            }
        }




        // GET: BebanMengajarController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BebanMengajarController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BebanMengajarController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: BebanMengajarController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BebanMengajarController/Edit/5
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

        // GET: BebanMengajarController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BebanMengajarController/Delete/5
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
