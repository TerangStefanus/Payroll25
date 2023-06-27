namespace Payroll25.Models
{
    public class IdentitasDosenModel
    {
        public string NPP { get; set; }
        public string NAMA { get; set; }
        public string NAMA_LENGKAP_GELAR { get; set; }

        //REMINDER TGL_LAHIR di SQL Server datetime
        public string TGL_LAHIR { get; set; }
        public string JNS_KEL { get; set; }
        public string AGAMA { get; set; }
        public int ID_UNIT { get; set; }
        public int ID_UNIT_AKADEMIK { get; set; }
        public string ID_REF_GOLONGAN { get; set; }
        public int ID_REF_JBTN_AKADEMIK { get; set; }
        public string NO_TELPON_HP { get; set; }
        public string NPWP { get; set; }
        public string ALAMAT { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }    
        public string PASSWORD_RIPEM { get; set; }

    }
}
