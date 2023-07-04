using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll25.Models
{
    public class IdentitasAsistenModel
    {
        public int ID_ASISTEN { get; set; }

        public int ID_TAHUN_AKADEMIK { get; set; }

        public int NO_SEMESTER { get; set; }

        public string NPM { get; set; }

        public int ID_UNIT { get; set; }
    }

    public class IndexViewModel
    {
        public IEnumerable<IdentitasAsistenModel> IdentitasAsistenList { get; set; }
        public IdentitasAsistenModel IdentitasAsisten { get; set; }
    }


}


