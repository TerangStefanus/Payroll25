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

        public async Task<IActionResult> Index(string prodi = null, string namaMK = null)
        {
            try
            {
                var viewModel = new BebanViewModel();

                // If both prodi and namaMK are provided
                if (!string.IsNullOrEmpty(prodi) && !string.IsNullOrEmpty(namaMK))
                {
                    viewModel.BebanMengajarList = await DAO.ShowBebanMengajarAsync(prodi, namaMK);
                }
                // If only prodi is provided
                else if (!string.IsNullOrEmpty(prodi))
                {
                    viewModel.BebanMengajarList = await DAO.ShowBebanMengajarAsync(prodi);
                }
                // If only namaMK is provided
                else if (!string.IsNullOrEmpty(namaMK))
                {
                    viewModel.BebanMengajarList = await DAO.ShowBebanMengajarAsync(null, namaMK);
                }
                // If none of the parameters are provided
                else
                {
                    viewModel.BebanMengajarList = await DAO.ShowBebanMengajarAsync();
                }

                viewModel.ProdiFilter = prodi;
                viewModel.NamaMKFilter = namaMK;

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
