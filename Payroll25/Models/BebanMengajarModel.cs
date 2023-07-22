namespace Payroll25.Models
{
    public class BebanMengajarModel
    {
        public string NPP { get; set; }
        public string KODE_MK { get; set; }
        public int SKS { get; set; }   
        public string NAMA_MK { get; set; }
        public string KELAS { get; set; }

        //Reminder TGL_AWAL_SK merupakan Datetime
        public string AWAL { get; set; }

        //Reminder TGL_AKHIR_SK merupakan Datetime
        public string AKHIR { get; set; }

        public class BebanViewModel
        {
            public IEnumerable<BebanMengajarModel> BebanMengajarList { get; set; }
            public BebanMengajarModel BebanMengajar { get;set;}
            public string ProdiFilter { get; set; }
            public string NamaMKFilter { get; set; }
            public string FakultasFilter { get; set; }
        }

    }
}
