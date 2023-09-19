using Microsoft.AspNetCore.Mvc;
using Payroll25.DAO;
using Payroll25.Models;
using Rotativa.AspNetCore;
using static Payroll25.Models.DataGajiModel;

namespace Payroll25.Controllers
{
    public class DataGajiController : Controller
    {
        private readonly DataGajiDAO DAO;

        public DataGajiController()
        {
            DAO = new DataGajiDAO();
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var unitList = await DAO.ListUnit();

                var viewModel = new DataGajiModel.DataGajiViewModel
                {
                    DataGajiList = unitList
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan saat mengambil data: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CetakSlipGaji(DataGajiViewModel viewModel)
        {
            var dataUnit = await DAO.GetUnit(viewModel.DataGaji.ID_UNIT);
            var dataBulan = await DAO.GetBulan(viewModel.DataGaji.ID_BULAN);
            var dataKaryawan = await DAO.GetDataKaryawan(viewModel.DataGaji.ID_PENGGAJIAN);
            var detailGaji = await DAO.GetDetailGaji(viewModel.DataGaji.ID_PENGGAJIAN);

            // Set tahun dari input pengguna
            foreach (var data in dataUnit)
            {
                data.TAHUN = viewModel.DataGaji.TAHUN;
            }

            // Gabungkan dataUnit, dataBulan, dan dataKaryawan ke dalam satu model untuk dikirim ke view
            var combinedData = dataUnit.Select(u => new DataGajiModel
            {
                // ... set properti lainnya
                NAMA_UNIT = u.NAMA_UNIT,
                BULAN = dataBulan.FirstOrDefault()?.BULAN,
                TAHUN = u.TAHUN,
                NPP = dataKaryawan.FirstOrDefault()?.NPP,
                NAMA = dataKaryawan.FirstOrDefault()?.NAMA,
                GOLONGAN = dataKaryawan.FirstOrDefault()?.GOLONGAN,
                JENJANG = dataKaryawan.FirstOrDefault()?.JENJANG,
                NPWP = dataKaryawan.FirstOrDefault()?.NPWP,
                NO_REKENING = dataKaryawan.FirstOrDefault()?.NO_REKENING,
                NAMA_BANK = dataKaryawan.FirstOrDefault()?.NAMA_BANK,
                NAMA_REKENING = dataKaryawan.FirstOrDefault()?.NAMA_REKENING,
                DetailGaji = detailGaji 
            }).ToList();


            return new ViewAsPdf("CetakSlipGaji", combinedData)
            {
                FileName = "SlipGaji.pdf"
            };
        }









    }
}
