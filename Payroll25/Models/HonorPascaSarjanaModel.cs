namespace Payroll25.Models
{
    public class HonorPascaSarjanaModel
    {
        public int ID_VAKASI { get; set; }
        public string NPP { get; set; }
        public string NAMA { get; set; }
        public string KEGIATAN { get; set; }// Komponen Gaji di tabel MST_KOMPONEN_GAJI
        public float Jml_Sks { get; set; }

        //Reminder DATE_INSERTED merupakan Datetime
        public string Tgl_buat { get; set; }

        //<!-- Create Model Vakasi Honorarium Ujian Reguler -->
        public int ID_KOMPONEN_GAJI { get; set; }

        public int ID_BULAN_GAJI { get; set; }

        public int GET_BULAN_GAJI { get; set; }// Untuk Dropdown GetBulanGaji

        //public string NPP { get; set; } sudah ada 

        public float JUMLAH { get; set; }
        public DateTime DATE_INSERTED { get; set; } // Auto isi pada hari di input
        public string DESKRIPSI { get; set; }

        //<!-- Create Model Vakasi Honorarium Ujian Reguler -->

        public class HonorPascaSarjanaViewModel
        {
            public IEnumerable<HonorPascaSarjanaModel> HonorPascaSarjanaList { get; set; }
            public HonorPascaSarjanaModel HonorPascaSarjana { get; set; }
        }

    }
}
