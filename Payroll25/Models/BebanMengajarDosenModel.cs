namespace Payroll25.Models
{
    public class BebanMengajarDosenModel
    {
        // -------- CODE INPUT FORM -------- \\
        public int ID_TAHUN_AKADEMIK { get; set; }
        public int NO_SEMESTER { get; set; }
        public string NPP { get; set; }
        public int TOTAL_SKS { get; set; }
        public int BEBAN_STRUKTURAL { get; set; }
        public int KELEBIHAN_BEBAN { get; set; }
        public DateTime TGL_AWAL_SK { get; set; }
        public DateTime TGL_AKHIR_SK { get; set; }

        // -------- CODE INPUT FORM -------- //

        public int ID_BEBAN_MENGAJAR { get; set; }
        public string NAMA { get; set; } // diambil dari MST KARYAWAN
        public string BEBAN_GELAR { get; set; } // NAMA_TARIF_PAYROLL 
        public float TARIF { get; set; } // NOMINAL
        public float TOTAL_NOMINAL { get; set; } // ini adalah sebuah model untuk menyimpan data sementara 

        public class BebanMengajarDosenViewModel
        {
            public IEnumerable<BebanMengajarDosenModel> BebanMengajarDosenList { get; set; }
            public BebanMengajarDosenModel BebanMengajarDosen { get; set; }
            public string NPPFilter { get; set; }
            public int? TAHUNFilter { get; set; }
            public string NAMAFilter { get; set; }
        }

    }
    
    public class DBOutput
    {
        public bool status { get; set; }
        public string pesan { get; set; }
        public dynamic data { get; set; }
    }
}
