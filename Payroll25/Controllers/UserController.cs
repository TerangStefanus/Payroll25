using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Security.Cryptography;
using System.Text;


namespace Payroll25.Controllers
{

	public class UserController : Controller
	{

        AccountDAO DAO;

        public UserController()
        {
            DAO = new AccountDAO();
        }

        [Authorize(Roles = "Mahasiswa")]
        public IActionResult Index_User()
        {
            return View();
        }

        [Authorize(Roles = "Dosen Kontrak")] 
        public IActionResult Index_Dosen()
        {
            return View();
        }

        [Authorize(Roles = "Pelatih")]
        public IActionResult Index_Pelatih()
        {
            return View();
        }
    }
}
