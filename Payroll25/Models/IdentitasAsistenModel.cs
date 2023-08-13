using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll25.Models
{
    public class IdentitasAsistenModel
    {
        public int ID_ASISTEN { get; set; }

        public int ID_TAHUN_AKADEMIK { get; set; }

        public int NO_SEMESTER { get; set; }

        public string NPM { get; set; }

        public string NAMA_MHS { get; set; }

        public int ID_UNIT { get; set; }

        public string NAMA_UNIT { get; set; } 

        public string NO_REKENING { get; set; }

        public string NAMA_REKENING { get;set; }

        public string NAMA_BANK { get; set; }

        public int ID_JENIS_ASISTEN { get; set; }

        public string JENIS { get; set; }

    }

    public class IndexViewModel
    {
        public IEnumerable<IdentitasAsistenModel> IdentitasAsistenList { get; set; }
        public IdentitasAsistenModel IdentitasAsisten { get; set; }
        public List<IdentitasAsistenModel> UnitsList { get; set; }
        public List<IdentitasAsistenModel> JenisAsistenList { get; set; }
    }


}


