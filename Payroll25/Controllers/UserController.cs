using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Security.Cryptography;
using System.Text;


namespace Payroll25.Controllers
{
	[Authorize(Roles = "Mahasiswa")]
	public class UserController : Controller
	{

        AccountDAO DAO;

        public UserController()
        {
            DAO = new AccountDAO();
        }

        public IActionResult Index_User()
		{
			return View();
		}


	}
}
