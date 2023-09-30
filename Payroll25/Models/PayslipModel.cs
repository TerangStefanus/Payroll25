namespace Payroll25.Models
{
    public class PayslipModel
    {
        public string NPM { get; set; }

    }

    public class HeaderPenggajianUserAsistenModel
    {
        public int ID_PENGGAJIAN { get; set; }
        public string NPP { get; set; }
        public string NAMA { get; set; }
        public string GOLONGAN { get; set; }
        public string JENJANG { get; set; }
        public string NPWP { get; set; }
        public string NO_TABUNGAN { get; set; }
        public string NAMA_BANK { get; set; }
        public string NAMA_REKENING { get; set; }
        public string NAMA_UNIT { get; set; }
        public string JENIS { get; set; }

    }

    public class BodyDetailPenggajianUserAsistenModel
    {
        public int ID_PENGGAJIAN { get; set; }
        public int ID_KOMPONEN_GAJI { get; set; }
        public string NAMA_KOMPONEN_GAJI { get; set; }
        public float? JUMLAH_SATUAN { get; set; }
        public float? NOMINAL { get; set; }
        public string JENIS { get; set; }

    }

    public class SlipUserAsistenViewModel
    {
        public HeaderPenggajianUserAsistenModel Header { get; set; }
        public IEnumerable<BodyDetailPenggajianUserAsistenModel> Body { get; set; }
        public decimal TotalPenerimaanKotor { get; set; }
        public decimal TotalPajak { get; set; }
        public decimal TotalPenerimaanBersih { get; set; }
    }


}
