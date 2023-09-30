using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using Rotativa.AspNetCore;
using System.IO.Compression;

namespace Payroll25.Controllers
{
    public class PayslipController : Controller
    {
        private readonly PayslipDAO DAO;

        public PayslipController()
        {
            DAO = new PayslipDAO();
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
        public async Task<IActionResult> CheckAutoCetakSlipGajiMhs(int idBulanGaji, string npp)
        {
            var isAvailable = await DAO.CheckDataGajiMhs(idBulanGaji, npp);
            if (!isAvailable)
            {
                return NotFound(new { success = false, message = "Data gaji tidak tersedia" });
            }
            return Ok(new { success = true, message = "Data gaji tersedia" });
        }

        [HttpGet]
        public async Task<IActionResult> CetakSlipGajiAsisten(int idBulanGaji, string npp)
        {
            var headers = await DAO.GetHeaderPenggajianAsisten(idBulanGaji, npp);
            string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            if (headers == null || !headers.Any())
            {
                return NotFound(new { success = false, message = "Tidak ada data yang bisa dicetak." });
            }

            Directory.CreateDirectory(tempFolder);
            foreach (var header in headers)
            {
                var isDetailAvailable = await DAO.CheckDetailGajiAsisten(header.ID_PENGGAJIAN);
                if (!isDetailAvailable)
                {
                    return NotFound(new { success = false, message = "Tidak ada detail gaji yang bisa dicetak." });
                }

                var body = await DAO.GetBodyPenggajianAsisten(header.ID_PENGGAJIAN);

                decimal totalPenerimaanKotor = 0;
                decimal totalPajak = 0;

                foreach (var item in body)
                {
                    totalPenerimaanKotor += (decimal)item.NOMINAL.GetValueOrDefault();
                }

                totalPajak = totalPenerimaanKotor * 0.03m;  // Misalnya pajak adalah 3%
                decimal totalPenerimaanBersih = totalPenerimaanKotor - totalPajak;

                var model = new SlipUserAsistenViewModel
                {
                    Header = header,
                    Body = body,
                    TotalPenerimaanKotor = totalPenerimaanKotor,
                    TotalPajak = totalPajak,
                    TotalPenerimaanBersih = totalPenerimaanBersih
                };

                var pdf = new ViewAsPdf("SlipGajiUserAsisten", model)
                {
                    FileName = $"SlipGaji{header.JENIS}_{header.NPP}.pdf"
                };

                var pdfFile = await pdf.BuildFile(ControllerContext);
                System.IO.File.WriteAllBytes(Path.Combine(tempFolder, $"SlipGaji{header.JENIS}_{header.NPP}.pdf"), pdfFile);
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
