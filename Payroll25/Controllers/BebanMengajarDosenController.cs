﻿using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> Index(string NPPFilter = null, int? TAHUNFilter = null, string NAMAFilter = null)
        {
            if (TAHUNFilter == 0)
            {
                TAHUNFilter = null;
            }

            try
            {
                var bebanMengajarDosenList = await DAO.ShowBebanMengajarAsync(NPPFilter, TAHUNFilter,NAMAFilter) ?? new List<BebanMengajarDosenModel>();

                var viewModel = new BebanMengajarDosenModel.BebanMengajarDosenViewModel
                {
                    BebanMengajarDosenList = bebanMengajarDosenList,
                    NPPFilter = NPPFilter,
                    TAHUNFilter = TAHUNFilter,
                    NAMAFilter = NAMAFilter,
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

        [HttpPost]
        public IActionResult DeleteBebanMengajarDosen([FromBody] List<BebanMengajarDosenModel> model)
        {
            DBOutput data = new DBOutput();
            var success = 0;

            success = DAO.DeleteBebanMengajarDosen(model);
            if (success != 0)
            {
                data.status = true;
                data.pesan = " Delete data berhasil ";
            }
            else
            {
                data.status = false;
                data.pesan = " Delete data gagal";
            }

            return Json(data);
        }



    }
}
