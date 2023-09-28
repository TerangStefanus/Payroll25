namespace Payroll25.Models
{
    public class PenggajianPelatihModel
    {
        public int ID_PENGGAJIAN { get; set; }
        public string NPP { get; set; }
        public int ID_BULAN_GAJI { get; set; }
        public string NAMA { get; set; }
        public string STATUS_KEPEGAWAIAN { get; set; }
        public string MASA_KERJA_RIIL { get; set; }
        public string MASA_KERJA_GOL { get; set; }
        public string JBT_STRUKTURAL { get; set; }
        public string JBT_AKADEMIK { get; set; }
        public string JBT_FUNGSIONAL { get; set; }
        public string PANGKAT { get; set; }
        public string GOLONGAN { get; set; }
        public string? JENJANG { get; set; }
        public string NO_TABUNGAN { get; set; }
        public string NPWP { get; set; }  
    }

    public class PelatihDataModel
    {
        public int ID_PELATIH { get; set; }
        public int ID_TAHUN_AKADEMIK { get; set; }
        public int NO_SEMESTER { get; set; }
        public string NPP { get; set; }
        public string NAMA { get; set; }
        public int ID_UNIT { get; set; }
        public string NAMA_UNIT { get; set; }
        public string NO_REKENING { get; set; }
        public string NAMA_REKENING { get; set; }
        public string NAMA_BANK { get; set; }
    }

    public class KomponenGajiPelatihModel
    {
        public int ID_BULAN_GAJI { get; set; }
        public string NPP { get; set; }
        public int ID_KOMPONEN_GAJI { get; set; }
        public string NAMA_KOMPONEN_GAJI { get; set; }
        public int JUMLAH { get; set; }
        public float? TARIF { get; set; }
        public string JENIS { get; set; }
    }

    public class DetailPenggajianPelatihModel
    {
        public int ID_PENGGAJIAN { get; set; }
        public int ID_KOMPONEN_GAJI { get; set; }
        public string NAMA_KOMPONEN_GAJI { get; set; }
        public float? JUMLAH_SATUAN { get; set; }
        public float? NOMINAL { get; set; }
        public string JENIS { get; set; }
    }

    public class HeaderPenggajianPelatih
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

    public class SlipGajiPelatihViewModel
    {
        public HeaderPenggajianPelatih Header { get; set; }
        public IEnumerable<DetailPenggajianPelatihModel> Body { get; set; }
        public decimal TotalPenerimaanKotor { get; set; }
        public decimal TotalPajak { get; set; }
        public decimal TotalPenerimaanBersih { get; set; }
    }


}
