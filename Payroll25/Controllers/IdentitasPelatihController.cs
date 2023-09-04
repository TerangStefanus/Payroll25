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

        // GET: IdentitasAsisten
        public IActionResult Index()
        {
            var viewModel = new IdentitasPelatihViewModel
            {
                IdentitasPelatihList = DAO.ShowIdentitasPelatih(),
                IdentitasPelatih = new IdentitasPelatihModel(),
            };

            return View(viewModel);
        }
    }
}
