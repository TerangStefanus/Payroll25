using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;

namespace Payroll25.Models
{
    public class PenggajianModel
    {

        [Required]
        [Name("ID_PENGGAJIAN")]
        public int? ID_PENGGAJIAN { get; set; }
        
        [Optional]
        public string NPP { get; set; }

        [Optional]
        public int? ID_BULAN_GAJI { get; set; }

        [Optional]
        public string NAMA { get; set; }

        [Optional]
        public string STATUS_KEPEGAWAIAN { get; set; }

        [Optional]
        public string MASA_KERJA_RIIL { get; set; }

        [Optional]
        public string MASA_KERJA_GOL { get; set; }

        [Optional]
        public string JBT_STRUKTURAL { get; set; }
        
        [Optional]
        public string JBT_AKADEMIK { get; set; }
        
        [Optional]
        public string JBT_FUNGSIONAL { get; set; }

        [Optional]
        public string PANGKAT { get; set; }

        [Optional]
        public string GOLONGAN { get; set; }

        [Optional]
        public string JENJANG { get; set; }

        [Optional]
        public string NO_TABUNGAN { get; set; }

        [Optional]
        public string NPWP { get; set; }

        [Optional]
        public string NAMA_KOMPONEN_GAJI { get; set; }

        [Optional]
        public int? JUMLAH { get; set; }

        [Optional]
        public float? TARIF { get; set; }

        [Required]
        [Name("NOMINAL")]
        public float? NOMINAL { get; set; }

        [Required]
        [Name("JUMLAH_SATUAN")]
        public float? JUMLAH_SATUAN { get; set; }

        [Required]
        [Name("ID_KOMPONEN_GAJI")]
        public int? ID_KOMPONEN_GAJI { get; set; }


        public class PenggajianViewModel
        {
            public IEnumerable<PenggajianModel> PenggajianList { get; set; }
            public IEnumerable<PenggajianModel> KontrakPenggajianList { get; set; }
            public PenggajianModel Penggajian { get; set; }
            public string NPPFilter { get; set; }
            public string NAMAFilter { get; set; }
            public string UNITFilter { get; set; }
            public int BULANFilter { get; set; }

            [Required]
            [Display(Name = "CSV File")]
            [DataType(DataType.Upload)]
            [FileExtensions(Extensions = "csv", ErrorMessage = "Please upload a valid CSV file.")]
            public IFormFile CsvFile { get; set; }
        }


        [Optional]
        public int GET_BULAN_GAJI { get; set; }
        [Optional]
        public string KOMPONEN_GAJI { get; set; }

    }

    public class PenggajianModelMap : ClassMap<PenggajianModel>
    {
        public PenggajianModelMap()
        {
            Map(m => m.ID_PENGGAJIAN).Name("ID_PENGGAJIAN");
            Map(m => m.ID_KOMPONEN_GAJI).Name("ID_KOMPONEN_GAJI");
            Map(m => m.JUMLAH_SATUAN).Name("JUMLAH_SATUAN");
            Map(m => m.NOMINAL).Name("NOMINAL");
        }
    }
}
