namespace Payroll25.Models
{
    public class IdentitasDosenModel
    {
        public string NPP { get; set; }
        public string NAMA { get; set; }
        //REMINDER TGL_LAHIR di SQL Server datetime
        public string TGL_LAHIR { get; set; }
        public string JNS_KEL { get; set; }
        public int ID_UNIT { get; set; }
        public string ID_REF_GOLONGAN { get; set; }
        public string NO_TELPON_HP { get; set; }
        public string NPWP { get; set; }
        public string ALAMAT { get; set; }
        public string NO_REKENING { get; set; }
        public string NAMA_BANK { get; set; }   
        public string NAMA_REKENING { get;set; }
        public string STATUS_REKENING { get; set; }
        public string STATUS { get;set; }

        public class IdentitasDosenViewModel
        {
            public IEnumerable<IdentitasDosenModel> IdentitasDosenList { get; set; }
            public IdentitasDosenModel IdentitasDosen { get; set; }

            public List<IdentitasDosenModel> NPPList { get; set; }
        }

    }
}
