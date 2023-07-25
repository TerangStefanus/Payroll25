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
                var viewModel = new TunjanganViewModel();

                // Jika terdapat input prodi & namaMK & fakultas lakukan tampilkan model ini 
                if (!string.IsNullOrEmpty(prodi) && !string.IsNullOrEmpty(namaMK) && !string.IsNullOrEmpty(fakultas))
                {
                    viewModel.TunjanganPengabdianList = await DAO.ShowTunjanganPengabdianAsync(prodi, namaMK, fakultas);
                }
                // Jika terdapat input prodi & namaMK lakukan tampilkan model ini 
                else if (!string.IsNullOrEmpty(prodi) && !string.IsNullOrEmpty(namaMK))
                {
                    viewModel.TunjanganPengabdianList = await DAO.ShowTunjanganPengabdianAsync(prodi, namaMK);
                }
                // Jika terdapat input prodi & fakultas lakukan tampilkan model ini 
                else if (!string.IsNullOrEmpty(prodi) && !string.IsNullOrEmpty(fakultas))
                {
                    viewModel.TunjanganPengabdianList = await DAO.ShowTunjanganPengabdianAsync(prodi, fakultas);
                }
                // Jika terdapat input namaMK & fakultas lakukan tampilkan model ini 
                else if (!string.IsNullOrEmpty(namaMK) && !string.IsNullOrEmpty(fakultas))
                {
                    viewModel.TunjanganPengabdianList = await DAO.ShowTunjanganPengabdianAsync(namaMK, fakultas);
                }
                // Jika terdapat input prodi lakukan tampilkan model ini 
                else if (!string.IsNullOrEmpty(prodi))
                {
                    viewModel.TunjanganPengabdianList = await DAO.ShowTunjanganPengabdianAsync(prodi);
                }
                // Jika terdapat input namaMK lakukan tampilkan model ini 
                else if (!string.IsNullOrEmpty(namaMK))
                {
                    viewModel.TunjanganPengabdianList = await DAO.ShowTunjanganPengabdianAsync(null, namaMK);
                }
                // Jika terdapat input namaMK lakukan tampilkan model ini 
                else if (!string.IsNullOrEmpty(fakultas))
                {
                    viewModel.TunjanganPengabdianList = await DAO.ShowTunjanganPengabdianAsync(null, null, fakultas);
                }
                // Jika tidak terdapat input tampilkan model ini
                else
                {
                    viewModel.TunjanganPengabdianList = await DAO.ShowTunjanganPengabdianAsync();
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

        // GET: TunjanganPengabdianController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TunjanganPengabdianController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TunjanganPengabdianController/Create
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

        // GET: TunjanganPengabdianController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TunjanganPengabdianController/Edit/5
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

        // GET: TunjanganPengabdianController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TunjanganPengabdianController/Delete/5
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
