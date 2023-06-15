using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Payroll25.Controllers
{
    public class AccountController : Controller
    {

        AccountDAO accDAO;

        public AccountController()
        {
            accDAO = new AccountDAO();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult LoginKaryawan(LoginModel obj)
        {
            ClaimIdentity identity;
            var dataKary = accDAO.GetAdmin(obj.USERNAME);
            if(dataKary != null)
            {
                var hashpass = _encPassMhs(obj.PASSWORD);
                if(hashpass == dataKary.PASSWORD)
                {
                    identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.NameIdentifier , dataKary.USERNAME),
                        new Claim(ClaimTypes.Name, dataKary.NAMA),
                        new Claim(ClaimTypes.Role, dataKary.ROLE),
                        new Claim("user", dataKary.NPP),
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    HttpContext.SignInAsync(
                       CookieAuthenticationDefaults.AuthenticationScheme,
                       new ClaimsPrincipal(identity));
                }
                
            }
        }

        private string _encPassMhs(string password)
        {
            //password to RIPEMD160
            string hash = "";
            Encoding enc = Encoding.GetEncoding(1252);
            RIPEMD160 ripemdHasher = RIPEMD160.Create();
            byte[] data = ripemdHasher.ComputeHash(Encoding.Default.GetBytes(password));
            hash = enc.GetString(data);

            return hash;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
