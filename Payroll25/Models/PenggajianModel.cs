using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;

namespace Payroll25.Models
{
    public class PenggajianModel
    {

        [Optional]
        public int? ID_PENGGAJIAN { get; set; }
        
        [Required]
        public string NPP { get; set; }

        [Required]
        public int? ID_BULAN_GAJI { get; set; }

        [Required]
        public string NAMA { get; set; }

        [Required]
        public string STATUS_KEPEGAWAIAN { get; set; }

        [Optional]
        public string MASA_KERJA_RIIL { get; set; }

        [Optional]
        public string MASA_KERJA_GOL { get; set; }

        [Required]
        public string JBT_STRUKTURAL { get; set; }
        
        [Required]
        public string JBT_AKADEMIK { get; set; }
        
        [Required]
        public string JBT_FUNGSIONAL { get; set; }

        [Required]
        public string PANGKAT { get; set; }

        [Required]
        public string GOLONGAN { get; set; }

        [Required]
        public string JENJANG { get; set; }

        [Required]
        public string NO_TABUNGAN { get; set; }

        [Required]
        public string NPWP { get; set; }

        [Optional]
        public string NAMA_KOMPONEN_GAJI { get; set; }

        [Optional]
        public int? JUMLAH { get; set; }

        [Optional]
        public float? TARIF { get; set; }

        [Optional]
        public float? NOMINAL { get; set; }


        public class PenggajianViewModel
        {
            public IEnumerable<PenggajianModel> PenggajianList { get; set; }
            public IEnumerable<PenggajianModel> KontrakPenggajianList { get; set; }
            public PenggajianModel Penggajian { get; set; }
            public string NPPFilter { get; set; }
            public string NAMAFilter { get; set; }
            public string UNITFilter { get; set; }
            public int BULANFilter { get; set; }
        }

        public int GET_BULAN_GAJI { get; set; }
        public int ID_KOMPONEN_GAJI { get;set; }

    }
}
