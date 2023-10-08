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

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult LoginAct(string username, string password)
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            bool isAuth = false;
            var data = DAO.GetKaryawan(username);
            var dataMHS = DAO.GetMahasiswa(username);
            var dataDosenKontrak = DAO.GetDosenKontrak(username);
            var dataPelatih = DAO.GetPelatih(username);


            if (data != null ) 
            {
                //Data dicek masuk
                var hashpass = _encPassMhs(password);// password di enkripsi menggunakan RIPEMD160
                if(hashpass == data.password_ripem) 
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
            else if(dataMHS != null)
            {
                bool isAsisten = DAO.CheckAsisten(username);
                if (!isAsisten)
                {
                    TempData["error"] = "NPM bukan Student Staff / Asisten";
                    return RedirectToAction("Login");
                }

                //Data dicek masuk
                var hashpass = _encPassMhs(password);// password di enkripsi menggunakan RIPEMD160
                if (hashpass == dataMHS.PASSWORD)
                {
                    isAuth = true;
                    identity = new ClaimsIdentity(new[] {
                               new Claim(ClaimTypes.Name, dataMHS.NPM),
                               new Claim(ClaimTypes.Name, dataMHS.NAMA_MHS),
                               new Claim(ClaimTypes.Role, dataMHS.ROLE),
                               new Claim("username", dataMHS.NPM),
                               new Claim("name_mhs", dataMHS.NAMA_MHS),
                               new Claim("no_hp",dataMHS.NO_HP),
                               new Claim("tgl_lahir",dataMHS.TGL_LAHIR),
                               new Claim("alamat",dataMHS.ALAMAT),
                               new Claim("role", dataMHS.ROLE),

                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    HttpContext.SignInAsync(
                       CookieAuthenticationDefaults.AuthenticationScheme,
                       new ClaimsPrincipal(identity));
                }
            }
            else if (dataDosenKontrak != null) 
            {
                var hashpass = _encPassMhs(password); 
                if (hashpass == dataDosenKontrak.PASSWORD_RIPEM)
                {
                    isAuth = true;
                    identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, dataDosenKontrak.NPP),
                        new Claim(ClaimTypes.Name, dataDosenKontrak.NAMA),
                        new Claim(ClaimTypes.Role, dataDosenKontrak.ROLE),
                        new Claim("username", dataDosenKontrak.NPP),
                        new Claim("name_dosen", dataDosenKontrak.NAMA),
                        new Claim("role", dataDosenKontrak.ROLE),
                        new Claim("tgl_lahir", dataDosenKontrak.TGL_LAHIR.ToString()),
                        new Claim("alamat", dataDosenKontrak.ALAMAT),
                        new Claim("no_hp", dataDosenKontrak.NO_TELPON_HP)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity));
                }
                else
                {
                    TempData["error"] = "Password Salah!";
                }
            }
            else if (dataPelatih != null)
            {
                var hashpass = _encPassMhs(password);
                if (hashpass == dataPelatih.PASSWORD)
                {
                    isAuth = true;
                    identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, dataPelatih.NPP),
                        new Claim(ClaimTypes.Name, dataPelatih.NAMA),
                        new Claim(ClaimTypes.Role, dataPelatih.ROLE),
                        new Claim("username", dataPelatih.NPP),
                        new Claim("name_pelatih", dataPelatih.NAMA),
                        new Claim("role", dataPelatih.ROLE),
                        new Claim("tgl_lahir", dataPelatih.TGL_LAHIR.ToString()),
                        new Claim("alamat", dataPelatih.ALAMAT),
                        new Claim("no_hp", dataPelatih.NO_TELP)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity));
                }
                else
                {
                    TempData["error"] = "Password Salah!";
                }
            }
            else
            {
                TempData["error"] = "Data Karyawan Non Tetap tidak ditemukan";
            }



            if(data != null) 
            {
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

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else if(dataMHS != null)
            {
                if (isAuth && dataMHS.ROLE == "Mahasiswa")
                {
                    var principal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index_User", "User");
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else if (dataDosenKontrak != null) 
            {
                if (isAuth && dataDosenKontrak.ROLE == "Dosen Kontrak") 
                {
                    var principal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index_Dosen", "User"); 
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else if (dataPelatih != null) 
            {
                if (isAuth && dataPelatih.ROLE == "Pelatih") 
                {
                    var principal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index_Pelatih", "User");
                }
                else
                {
                    return RedirectToAction("Login");
                }
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

		public async Task<RedirectToActionResult> Logout()
		{
			// Clear the existing external cookie
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login");
		}


	}
}
