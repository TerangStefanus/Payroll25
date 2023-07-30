using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System;
using System.Threading.Tasks;
using static Payroll25.Models.BebanMengajarModel;
using static Payroll25.Models.TunjanganPengabdianModel;

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
                var viewModel = new BebanViewModel
                {
                    BebanMengajarList = await DAO.ShowBebanMengajarAsync(prodi, namaMK, fakultas),
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
