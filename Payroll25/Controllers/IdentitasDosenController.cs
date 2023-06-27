using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

namespace Payroll25.Controllers
{
    //[Authorize(Roles = "KSDM")]
    public class IdentitasDosenController : Controller
    {
        IdentitasDosenDAO DAO;

        public IdentitasDosenController()
        {
            DAO = new IdentitasDosenDAO();
        }

        public IActionResult Show_Index_IndentitasDosen()
        {
            var data = DAO.ShowIdentitasDosen();
            return View(data);
        }

    }
}
