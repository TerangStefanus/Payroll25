namespace Payroll25.Models
{
    public class TunjanganPengabdianModel
    {
        public string NPP { get; set; }
        public int SKS { get; set; }
        public string NAMA_MK { get; set; }
        public string KELAS { get; set; }
        public float Jml_Hadir { get; set; }

        //Reminder DATE_INSERTED merupakan Datetime
        public string Tgl_buat { get; set; }
        public string Kode_Unit { get; set; }


        //<!-- Create Model Vakasi -->
        public int ID_KOMPONEN_GAJI { get; set; }

        public int GET_KOMPONEN_GAJI { get; set; } // Untuk Dropdown GetKomponenGaji

        public int ID_BULAN_GAJI { get; set; }

        public int GET_BULAN_GAJI { get; set; }// Untuk Dropdown GetBulanGaji

        //public string NPP { get; set; } sudah ada 

        public float JUMLAH { get; set; }
        public DateTime DATE_INSERTED { get; set; } // Auto isi pada hari di input
        public string DESKRIPSI { get ; set; }

        //<!-- Create Model Vakasi -->


        public class TunjanganViewModel
        {
            public IEnumerable<TunjanganPengabdianModel> TunjanganPengabdianList { get; set; }
            public TunjanganPengabdianModel TunjanganPengabdian { get; set; }
            public string ProdiFilter { get; set; }
            public string NamaMKFilter { get; set; }
            public string FakultasFilter { get; set; }
        }

    }
}
