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
        public async Task<IActionResult> CheckAutoCetakSlipGaji(int idBulanGaji, string npp)
            {
            var isAvailable = await DAO.CheckDataGajiMhs(idBulanGaji, npp); // Data harus 1 NPM
            if (!isAvailable)
            {
                return NotFound(new { success = false, message = "Data gaji tidak tersedia" });
            }
            return Ok(new { success = true, message = "Data gaji tersedia" });
        }

        [HttpGet]
        public async Task<IActionResult> CetakSlipGaji(int idBulanGaji, string npp)
        {
            var asistenDataList = await DAO.GetAsistenDataByNPP(npp); // Data harusnya 1 NPM

            if (asistenDataList == null || !asistenDataList.Any())
            {
                return NotFound(new { success = false, message = "Tidak ada data asisten yang bisa dicetak." });
            }

            string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            var uniqueJenisAsisten = asistenDataList.Select(a => a.JENIS).Distinct();
            List<string> errors = new List<string>();

            foreach (var jenis in uniqueJenisAsisten)// Iterasi per jenis Asisten
            {
                var headers = await DAO.GetHeaderPenggajianUserAsisten(idBulanGaji, npp, jenis);// Nanti akan dibedakan melalui sebuah fungsi convert Jenis -> Pangkat

                if (headers == null || !headers.Any())
                {
                    errors.Add($"Tidak ada data yang bisa dicetak untuk jenis {jenis}.");
                    continue; // Skip to the next iteration
                }

                foreach (var header in headers)// Cetak data berdasarkan ID Penggajian per jenis mahasiswa ( Hasil Akurat kalau 1 NPM punya 2 (JENIS) lebih dari itu agak kacau kecuali ada perubahan data Vakasi Beban Mengajar ) 
                {
                    var isDetailAvailable = await DAO.CheckDetailGajiAsisten(header.ID_PENGGAJIAN);
                    if (!isDetailAvailable)
                    {
                        errors.Add($"Tidak ada detail gaji yang bisa dicetak untuk header {header.ID_PENGGAJIAN}.");
                        continue; // Skip to the next iteration
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

                    var model = new SlipUserAsistenViewModel
                    {
                        Header = header,
                        Body = body,
                        TotalPenerimaanKotor = totalPenerimaanKotor,
                        TotalPajak = totalPajak,
                        TotalPenerimaanBersih = totalPenerimaanBersih,
                        TandaTangan = await DAO.GetTandaTanganKSDM(),
                        NamaKepalaKSDM = await DAO.GetNamaKepalaKSDM()
                    };

                    var pdf = new ViewAsPdf("SlipGajiUserAsisten", model)
                    {
                        FileName = $"SlipGaji{header.JENIS}_{header.NPP}.pdf"
                    };

                    var pdfFile = await pdf.BuildFile(ControllerContext);
                    System.IO.File.WriteAllBytes(Path.Combine(tempFolder, $"SlipGaji{header.JENIS}_{header.NPP}.pdf"), pdfFile);
                }
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
            else if (errors.Any())
            {
                return NotFound(new { success = false, message = string.Join(" ", errors) });
            }
            else
            {
                return NotFound(new { success = false, message = "Tidak ada data yang bisa dicetak." });
            }
        }






    }
}
