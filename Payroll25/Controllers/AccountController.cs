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

        AccountDAO DAO;

        public AccountController()
        {
            DAO = new AccountDAO();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult LoginAct(string username, string password)
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            bool isAuth = false;
            var data = DAO.GetKaryawan(username);

            if(data != null)
            {
                //Data dicek masuk
                var hashpass = _encPassMhs(password);// password di enkripsi menggunakan RIPEMD160
                if(hashpass == data.password) 
                {
                    //Pengecekan Password aman
                    isAuth = true;
                    identity = new ClaimsIdentity(new[] {
                               new Claim(ClaimTypes.Name, data.nama),
                               new Claim(ClaimTypes.Name, data.deskripsi),
                               new Claim(ClaimTypes.Role, data.npp),
                               new Claim("username", data.npp),
                               new Claim("role", data.deskripsi),
                               new Claim("menu", GenerateMenu(data.deskripsi))
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    HttpContext.SignInAsync(
                       CookieAuthenticationDefaults.AuthenticationScheme,
                       new ClaimsPrincipal(identity));
                }
                else
                {
                    // Password salah
                    TempData["error"] = "Password Salah!";

                }
            }
            else
            {
                TempData["error"] = "Data Karyawan tidak ditemukan";
            }

            //return RedirectToAction("Index", "Home");

            if (isAuth && data.deskripsi == "KSDM")
            {
                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }

            else if (isAuth)
            {
                var principal = new ClaimsPrincipal(identity);
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("SlipGaji", "Home");
            }
            else
            {
                return RedirectToAction("Login");
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

        protected string GenerateMenu(string npp)
        {
            string menu = "";
            List<ModelMenu> menus = new List<ModelMenu>();
            List<ModelSubMenu> submenus = new List<ModelSubMenu>();

            menus = DAO.GetMenuKaryawan(npp);
            submenus = DAO.GetSubMenuKaryawan(npp);

            if (menu != null)
            {
                foreach (var row in menus)
                {
                    menu += $"<li class='nav-item'><a href = '#' class='nav-link'><i class='nav-icon fas fa-circle'></i><p>{row.DESKRIPSI}<i class='fas fa-angle-left right'></i></p></a><ul class='nav nav-treeview'>";
                    var filtersub = submenus.Where(x => x.ID_SI_MENU == row.ID_SI_MENU).ToList();

                    foreach (var submenu in filtersub)
                    {
                        menu += $"<li class='nav-item'><a href='{submenu.LINK}' class='nav-link'><i class='far fa-circle nav-icon'></i><p>{submenu.DESKRIPSI}</p></a></li>";
                    }

                    menu += "</ul></li> ";
                }
            }

            return menu;
        }

        public IActionResult Login()
        {
            return View();
        }
    }
}
