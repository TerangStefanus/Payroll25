namespace Payroll25.Models
{
    public class HonorPendadaranModel
    {
        
        // -------- CODE INPUT FORM -------- \\
        public int ID_KOMPONEN_GAJI { get; set; }
        public string NPP { get; set; }
        public string NAMA { get; set; }
        public int ID_BULAN_GAJI { get; set; }
        public float JUMLAH { get; set; }
        public DateTime DATE_INSERTED { get; set; } // Auto isi pada hari di input
        public string DESKRIPSI { get; set; }

        // -------- CODE INPUT FORM -------- \\

        public int ID_VAKASI { get; set; }
        public string KOMPONEN_GAJI { get; set; }
        public float NOMINAL { get; set; }
        public string TANGGAL { get; set; }
        public int GET_BULAN_GAJI { get; set; }// Untuk Dropdown GetBulanGaji
         
        
        public class HonorPendadaranViewModel
        {
            public IEnumerable<HonorPendadaranModel> HonorPendadaranList { get; set; }
            public HonorPendadaranModel HonorPendadaran { get; set; }
            public string NPPFilter { get; set; }
            public string NAMAFilter { get; set; }
        }

    }
}
