namespace Payroll25.Models
{
    public class PenggajianDosenModel
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

    public class GajiKaryawanModel
    {
        // Karyawan
        public string NPP { get; set; }
        public string NAMA_LENGKAP_GELAR { get; set; }
        public DateTime TGL_MASUK { get; set; }
        public int? ID_UNIT { get; set; }
        public int? MST_ID_UNIT { get; set; }
        public string ID_REF_GOLONGAN { get; set; }
        public string ID_REF_GOLONGAN_LOKAL { get; set; }
        public string NPWP { get; set; }
        public string STATUS_KEPEGAWAIAN { get; set; }
        public int? MASA_KERJA_GOLONGAN { get; set; }
        
        // Rekening 

        public string NO_REKENING { get; set; }
        public string NAMA_BANK { get; set; }
        public string NAMA_REKENING { get; set; }

    }

    public class KomponenGajiAndJumlahSatuanModel
    {
        public string NPP { get; set; }
        public int ID_KOMPONEN_GAJI { get; set; }
        public string NAMA_KOMPONEN_GAJI { get; set; }
        public int? JUMLAH { get; set; }
        public float? TARIF { get; set; }
        public int? ID_BULAN_GAJI { get; set; } 

    }

    public class DetailPenggajianModel
    {
        public int ID_PENGGAJIAN { get; set; }
        public int ID_KOMPONEN_GAJI { get; set; }
        public string NAMA_KOMPONEN_GAJI { get; set; }  
        public float? JUMLAH_SATUAN { get; set; }
        public float? NOMINAL { get; set; }
    }

    public class HeaderPenggajian
    {
        public int ID_PENGGAJIAN { get; set; }
        public string BULAN { get;set; }
        public int ID_TAHUN { get; set; }
        public string NPP { get; set; }
        public string NAMA { get; set; }
        public string GOLONGAN { get; set; }
        public string JENJANG { get; set; }
        public string NPWP { get; set; }
        public string NO_TABUNGAN { get; set; }
        public string NAMA_BANK { get; set; }
        public string NAMA_REKENING { get; set; }
        public string NAMA_UNIT { get; set; }
    }

    public class SlipGajiViewModel
    {
        public HeaderPenggajian Header { get; set; }
        public IEnumerable<DetailPenggajianModel> Body { get; set; }
        public decimal TotalPenerimaanKotor { get; set; }
        public decimal TotalPajak { get; set; }
        public decimal TotalPenerimaanBersih { get; set; }
    }



}
