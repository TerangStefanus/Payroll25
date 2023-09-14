using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;

namespace Payroll25.Models
{
    public class IdentitasPelatihModel
    {
        [Optional]
        public int? ID_PELATIH { get; set; }

        [Required]
        [StringLength(50)]
        [Name("NPP")]
        public string NPP { get; set; }

        [Required]
        [StringLength(50)]
        [Name("NAMA")]
        public string NAMA { get; set; }

        [Required]
        [Name("ID_TAHUN_AKADEMIK")]
        public int? ID_TAHUN_AKADEMIK { get; set; }

        [Required]
        [Name("NO_SEMESTER")]
        public int? NO_SEMESTER { get; set; }

        [Required]
        [Name("ID_UNIT")]
        public int? ID_UNIT { get; set; }

        [Optional]
        public string NAMA_UNIT { get; set; }

        [Required]
        [StringLength(50)]
        [Name("NO_REKENING")]
        public string NO_REKENING { get; set; }

        [Required]
        [StringLength(50)]
        [Name("NAMA_REKENING")]
        public string NAMA_REKENING { get; set; }

        [Required]
        [StringLength(50)]
        [Name("NAMA_BANK")]
        public string NAMA_BANK { get; set; }

        public class IdentitasPelatihViewModel
        {
            public IEnumerable<IdentitasPelatihModel> IdentitasPelatihList { get; set; }
            public IdentitasPelatihModel IdentitasPelatih { get; set; }
            public List<IdentitasPelatihModel> UnitsList { get; set; }

            [Required]
            [Display(Name = "CSV File")]
            [DataType(DataType.Upload)]
            [FileExtensions(Extensions = "csv", ErrorMessage = "Please upload a valid CSV file.")]
            public IFormFile CsvFile { get; set; }
        }

    }

    public class IdentitasPelatihModelMap : ClassMap<IdentitasPelatihModel>
    {
        public IdentitasPelatihModelMap()
        {
            Map(m => m.NPP).Name("NPP");
            Map(m => m.NAMA).Name("NAMA");
            Map(m => m.ID_TAHUN_AKADEMIK).Name("ID_TAHUN_AKADEMIK");
            Map(m => m.NO_SEMESTER).Name("NO_SEMESTER");
            Map(m => m.ID_UNIT).Name("ID_UNIT");
            Map(m => m.NO_REKENING).Name("NO_REKENING");
            Map(m => m.NAMA_REKENING).Name("NAMA_REKENING");
            Map(m => m.NAMA_BANK).Name("NAMA_BANK");
        }
    }


}

