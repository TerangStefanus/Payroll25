namespace Payroll25.Models
{
    public class IdentitasPelatihModel
    {
        public int ID_PELATIH { get; set; }

        public string NPP { get; set; }

        public string NAMA { get; set; }

        public int ID_TAHUN_AKADEMIK { get; set; }

        public int NO_SEMESTER { get; set; }

        public int ID_UNIT { get; set; }

        public string NAMA_UNIT { get; set; }

        public string NO_REKENING { get; set; }

        public string NAMA_REKENING { get; set; }

        public string NAMA_BANK { get; set; }

        public class IdentitasPelatihViewModel
        {
            public IEnumerable<IdentitasPelatihModel> IdentitasPelatihList { get; set; }
            public IdentitasPelatihModel IdentitasPelatih { get; set; }
            public List<IdentitasPelatihModel> UnitsList { get; set; }
        }

    }

    
}

