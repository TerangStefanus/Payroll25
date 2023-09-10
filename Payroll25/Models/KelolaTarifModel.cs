namespace Payroll25.Models
{
    public class KelolaTarifModel
    {
        // -------- CODE INPUT FORM -------- \\
        public int ID_MST_TARIF_PAYROLL { get; set; }
        public int ID_REF_JBTN_AKADEMIK { get; set; }
        public int ID_REF_STRUKTURAL { get; set; }
        public string ID_REF_GOLONGAN { get; set; }
        public int ID_REF_FUNGSIONAL { get; set; }
        public int ID_REF_JENJANG {get; set; }
        public string NAMA_TARIF_PAYROLL { get; set; }
        public float NOMINAL { get; set; }
        public string JENIS { get; set; }
        public string JENJANG_KELAS { get; set; }
        public int ID_KOMPONEN_GAJI { get; set; }

        // -------- CODE INPUT FORM -------- \\



        public class KelolaTarifViewModel
        {
            public IEnumerable<KelolaTarifModel> KelolaTarifList { get; set; }
            public KelolaTarifModel KelolaTarif { get; set; }
            public string NAMAFilter { get; set; }
        }



    }
}
