using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using System.Diagnostics;
using static Payroll25.Models.BebanMengajarDosenModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;

namespace Payroll25.Controllers
{
    [Authorize]
    public class BebanMengajarDosenController : Controller
    {
        private readonly BebanMengajarDosenDAO DAO;

        public BebanMengajarDosenController()
        {
            DAO = new BebanMengajarDosenDAO();
        }

        // GET: BebanMengajarDosenController    
        public async Task<IActionResult> Index()
        {
            try
            {
                var bebanMengajarDosen = await DAO.ShowBebanMengajarAsync();

                if (bebanMengajarDosen == null)
                {
                    return NotFound("Data tidak ditemukan."); // 404 status code
                }

                // Kemas objek tunggal ke dalam list
                var bebanMengajarDosenList = new List<BebanMengajarDosenModel> { bebanMengajarDosen };

                var viewModel = new BebanMengajarDosenViewModel
                {
                    BebanMengajarDosenList = bebanMengajarDosenList
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan saat mengambil data: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult InsertBebanMengajar([FromBody] BebanMengajarDosenModel model)
        {
            DBOutput data = new DBOutput();
            var success = DAO.InsertBebanMengajar(model);

            if (success != 0)
            {
                data.status = true;
                data.pesan = "Insert berhasil!";
            }
            else
            {
                data.status = false;
                data.pesan = "Insert gagal!";
            }

            return Json(data);
        }

        [HttpPost]
        public IActionResult UpdateBebanMengajar([FromBody] List<BebanMengajarDosenModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.UpdateBebanMengajar(model);
            if (success != 0)
            {
                data.status = true;
                data.pesan = " Update berhasil ";
            }
            else
            {
                data.status = false;
                data.pesan = " Update gagal";
            }

            return Json(data);
        }

    }
}
