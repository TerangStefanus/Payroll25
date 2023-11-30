using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;
using ClosedXML.Excel;
using CsvHelper.Configuration;
using System.Runtime.Serialization;

namespace Payroll25.Models
{
    public class HonorSisipanModel
    {
        [Optional]
        public int ID_VAKASI { get; set; }

        [Required]
        [Name("ID_KOMPONEN_GAJI")]
        public int? ID_KOMPONEN_GAJI { get; set; }

        [Required]
        [Name("ID_BULAN_GAJI")]
        public int? ID_BULAN_GAJI { get; set; }

        [Required]
        [StringLength(50)]
        [Name("NPP")]
        public string NPP { get; set; }

        [Required]
        [Name("JUMLAH")]
        public float? JUMLAH { get; set; }

        [Optional]
        public DateTime? DATE_INSERTED { get; set; } = DateTime.Now;

        [Optional]
        public string? DESKRIPSI { get; set; }

        [Ignore] // CsvHelper akan mengabaikan properti ini saat membaca atau menulis CSV
        [IgnoreDataMember] // ClosedXML akan mengabaikan properti ini saat membaca atau menulis XLS
        public IXLWorksheet WorksheetReference { get; set; }

        public class HonorSisipanViewModel
        {
            public IEnumerable<HonorSisipanModel> HonorSisipanList { get; set; }
            public HonorSisipanModel HonorSisipan { get; set; }
            public string NPPFilter { get; set; }

            [Required]
            [Display(Name = "Excel File")] // Ubah label sesuai dengan jenis file
            [DataType(DataType.Upload)]
            [FileExtensions(Extensions = "xlsx", ErrorMessage = "Please upload a valid Excel file.")]
            public IFormFile ExcelFile { get; set; }
        }

        [Optional]
        public string KOMPONEN_GAJI { get; set; }

    }

    public class HonorSisipanModelMap : ClassMap<HonorSisipanModel>
    {
        public HonorSisipanModelMap()
        {
            Map(m => m.ID_KOMPONEN_GAJI).Name("ID_KOMPONEN_GAJI");
            Map(m => m.ID_BULAN_GAJI).Name("ID_BULAN_GAJI");
            Map(m => m.NPP).Name("NPP");
            Map(m => m.JUMLAH).Name("JUMLAH");
        }
    }
}
