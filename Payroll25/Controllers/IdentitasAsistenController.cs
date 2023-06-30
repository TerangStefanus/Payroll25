using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;

namespace Payroll25.Controllers
{
    public class IdentitasAsistenController : Controller
    {
        IdentitasAsistenDAO identitasAsistenDAO;

        public IdentitasAsistenController()
        {
            identitasAsistenDAO = new IdentitasAsistenDAO();
        }

        // GET: IdentitasAsistenController
        public ActionResult Index()
        {
            var data = identitasAsistenDAO.ShowIdentitasAssisten();
            return View(data);
        }

        // GET: IdentitasAsistenController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: IdentitasAsistenController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IdentitasAsistenController/Create
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
