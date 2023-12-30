﻿using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using Rotativa.AspNetCore;
using System.IO.Compression;

namespace Payroll25.Controllers
{
    public class PenggajianAsistenController : Controller
    {
        private readonly PenggajianAsistenDAO DAO;

        public PenggajianAsistenController()
        {
            DAO = new PenggajianAsistenDAO();

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetBulanGajiDropdown(int tahun)
        {
            try
            {
                var bulanGajiList = await DAO.GetBulanGaji(tahun);

                // Pastikan model yang dikembalikan sesuai dengan model yang digunakan di view
                var result = bulanGajiList.Select(k => new { id_bulan = k.GET_BULAN_GAJI });

                return Json(result);
            }
            catch (Exception)
            {
                // Handle exceptions here
                return BadRequest("Failed to get Bulan Gaji data.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnits()
        {
            var units = await DAO.GetUnits();
            return Json(units);
        }

        [HttpGet]
        public async Task<IActionResult> GetJenis()
        {
            var jenis = await DAO.GetJenis();
            return Json(jenis);
        }


        [HttpPost]
        public async Task<IActionResult> AutoInsertPenggajianAsisten(int idBulanGaji, string tahun, string unit, string jenis)
        {
            var result = await DAO.AutoInsertPenggajianAsisten(idBulanGaji, tahun, unit, jenis);
            return Ok(new { success = result });
        }

        [HttpPost]
        public async Task<IActionResult> AutoHitungGajiAsisten(int idBulanGaji, string tahun, string unit, string jenis)
        {
            try
            {
                bool isSuccess = await DAO.AutoHitungGajiAsisten(idBulanGaji, tahun, unit, jenis);

                if (isSuccess)
                {
                    return Ok(new { success = true, message = "Penggajian berhasil dihitung." });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Penggajian gagal dihitung. Data mungkin kosong atau terjadi kesalahan lain." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, new { message = "Terjadi kesalahan internal server.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckAutoCetakSlipGajiMhs(int idBulanGaji, string tahun, string unit, string jenis)
        {
            var isAvailable = await DAO.CheckDataGajiMhs(idBulanGaji);
            if (!isAvailable)
            {
                return NotFound(new { success = false, message = "Data gaji tidak tersedia" });
            }
            return Ok(new { success = true, message = "Data gaji tersedia" });
        }

        [HttpGet]
        public async Task<IActionResult> AutoCetakSlipGajiAsisten(int idBulanGaji, string tahun, string unit,string jenis)
        {
            var headers = await DAO.GetHeaderPenggajianAsisten(idBulanGaji, unit, jenis);
            string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempFolder);

            foreach (var header in headers)
            {
                var isDetailAvailable = await DAO.CheckDetailGajiAsisten(header.ID_PENGGAJIAN);
                if (!isDetailAvailable)
                {
                    continue;
                }

                var body = await DAO.GetBodyPenggajianAsisten(header.ID_PENGGAJIAN);

                decimal totalPenerimaanKotor = 0;
                decimal totalPajak = 0;

                var Tax = await DAO.GetTarifPajakByNPWPStatus(header.NPP);

                foreach (var item in body)
                {
                    totalPenerimaanKotor += (decimal)item.NOMINAL.GetValueOrDefault();
                }

                totalPajak = totalPenerimaanKotor * Tax;
                decimal totalPenerimaanBersih = totalPenerimaanKotor - totalPajak;

                var model = new SlipGajiAsistenViewModel
                {
                    Header = header,
                    Body = body,
                    TotalPenerimaanKotor = totalPenerimaanKotor,
                    TotalPajak = totalPajak,
                    TotalPenerimaanBersih = totalPenerimaanBersih,
                    TandaTangan = await DAO.GetTandaTanganKSDM(),
                    NamaKepalaKSDM = await DAO.GetNamaKepalaKSDM()
                };

                var pdf = new ViewAsPdf("SlipGajiAsisten", model)
                {
                    FileName = Path.Combine(tempFolder, $"SlipGaji_{header.NPP}.pdf")
                };

                var pdfFile = await pdf.BuildFile(ControllerContext);
                System.IO.File.WriteAllBytes(Path.Combine(tempFolder, $"SlipGaji_{header.NPP}.pdf"), pdfFile);
            }

            string zipPath = Path.Combine(Path.GetTempPath(), "SlipGaji.zip");
            ZipFile.CreateFromDirectory(tempFolder, zipPath);

            Directory.Delete(tempFolder, true);

            byte[] zipBytes = System.IO.File.ReadAllBytes(zipPath);
            System.IO.File.Delete(zipPath);

            if (zipBytes.Length > 0)
            {
                return File(zipBytes, "application/zip", "SlipGaji.zip");
            }
            else
            {
                return NotFound(new { success = false, message = "Tidak ada data yang bisa dicetak." });
            }
        }













    }
}
