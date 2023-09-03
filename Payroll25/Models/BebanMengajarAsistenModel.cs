namespace Payroll25.Models
{
    public class BebanMengajarAsistenModel
    {
        // -------- CODE INPUT FORM -------- \\
        public int ID_KOMPONEN_GAJI { get; set; }// auto isi 77 
        public int ID_BULAN_GAJI { get; set; } 
        public int GET_BULAN_GAJI { get; set; }// Untuk Dropdown GetBulanGaji
        public string NPP { get; set; } // Sebagai ganti NPM tapi digunakan Input
        public float JUMLAH { get; set; }
        public string DATE_INSERTED { get; set; } // auto isi Datetime.now
        public string DESKRIPSI { get; set; }
        
        // -------- CODE INPUT FORM -------- //

        public int ID_VAKASI { get; set; }
        public string NAMA_ASISTEN { get; set; }
        public string NPM { get; set; } // Menampilkan NPM pada Table
        public string JENIS_ASISTEN { get; set; }
        public string VAKASI { get; set; } // Kolom KOMPONEN_GAJI 
        public float TARIF { get; set; }
        public float TOTAL_NOMINAL { get; set; }

        public class BebanMengajarAsistenViewModel
        {
            public IEnumerable<BebanMengajarAsistenModel> BebanMengajarAsistenList { get; set; }
            public BebanMengajarAsistenModel BebanMengajarAsisten { get; set; }
            public string NPMFilter { get; set; }
            public string NAMAFilter { get; set; }
        }
    }
}
