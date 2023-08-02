namespace Payroll25.Models
{
    public class TunjanganPengabdianModel
    {
        public int ID_VAKASI { get; set; }
        public string NPP { get; set; }
        public int ID_BULAN_GAJI { get; set; }
        public float Jml_Hadir { get; set; }

        //Reminder DATE_INSERTED merupakan Datetime
        public string Tgl_buat { get; set; }
        public string DESKRIPSI { get; set; } 


                //<!-- Create Model Vakasi -->
                public int ID_KOMPONEN_GAJI { get; set; }

                //public int ID_BULAN_GAJI { get; set; } sudah ada

                public int GET_BULAN_GAJI { get; set; }// Untuk Dropdown GetBulanGaji

                //public string NPP { get; set; } sudah ada 

                public float JUMLAH { get; set; }
                public DateTime DATE_INSERTED { get; set; } // Auto isi pada hari di input

                //public string DESKRIPSI { get ; set; } sudah ada

                //<!-- Create Model Vakasi -->


        public class TunjanganViewModel
        {
            public IEnumerable<TunjanganPengabdianModel> TunjanganPengabdianList { get; set; }
            public TunjanganPengabdianModel TunjanganPengabdian { get; set; }
        }

    }
}
