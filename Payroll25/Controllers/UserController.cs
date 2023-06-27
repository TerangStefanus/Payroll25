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

		//public  IActionResult DetailProfile()
		//{
  //          //var nameMhsClaim = User.FindFirst("name_mhs");
  //          //var nameMhs = nameMhsClaim?.Value;
  //          //var noHPClaim = User.FindAll("no_hp");
  //          //var tgl_LahirClaim = User.FindAll("tgl_lahir");
  //          //var alamatClaim = User.FindAll("alamat");

  //          //// Mengisi model dengan nilai klaim yang ditemukan
  //          //var model = new MhsModel
  //          //{
  //          //    NameMhs = nameMhs,
  //          //    NoHp = noHPClaim.FirstOrDefault()?.Value,
  //          //    TglLahir = tgl_LahirClaim.FirstOrDefault()?.Value,
  //          //    Alamat = alamatClaim.FirstOrDefault()?.Value
  //          //};

  //          return View(model);
  //      }

	}
}
