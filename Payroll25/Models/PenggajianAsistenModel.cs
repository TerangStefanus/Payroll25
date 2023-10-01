namespace Payroll25.Models
{
    public class PenggajianAsistenModel
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
        public string JENIS { get; set; }

    }

    public class AsistenDataModel
    {
        public int ID_ASISTEN { get; set; }
        public int ID_TAHUN_AKADEMIK { get; set; }
        public int NO_SEMESTER { get; set; }
        public string NPM { get; set; }
        public string NAMA_MHS { get; set; }
        public int ID_UNIT { get; set; }
        public string NAMA_UNIT { get; set; }
        public string NO_REKENING { get; set; }
        public string NAMA_REKENING { get; set; }
        public string NAMA_BANK { get; set; }
        public string JENIS { get; set; }
    }

    public class KomponenGajiMhsModel
    {
        public int ID_BULAN_GAJI { get; set; }
        public string NPP { get; set; }
        public int ID_KOMPONEN_GAJI { get; set; }
        public string NAMA_KOMPONEN_GAJI { get; set; }
        public int JUMLAH { get; set; }
        public float? TARIF { get; set; }
        public string JENIS { get; set; }
    }

    public class DetailPenggajianMhsModel
    {
        public int ID_PENGGAJIAN { get; set; }
        public int ID_KOMPONEN_GAJI { get; set; }
        public string NAMA_KOMPONEN_GAJI { get; set; }
        public float? JUMLAH_SATUAN { get; set; }
        public float? NOMINAL { get; set; }
        public string JENIS { get; set; }
    }

    public class KomponenGajiDictionaryModel
    {
        public static readonly Dictionary<string, Dictionary<string, List<int>>> validKomponenGaji = new Dictionary<string, Dictionary<string, List<int>>>()
    {
        {
            "Asisten Mahasiswa", new Dictionary<string, List<int>>()
            {
                { "1", new List<int> { 77, 78, 79, 80, 167, 182, 194, 197, 199 } },
                // Tambahkan mapping untuk PANGKAT lainnya di sini
            }
        },
        {
            "Asisten Lab", new Dictionary<string, List<int>>()
            {
                { "2", new List<int> { 77,78,80,168, 200 } },
                // Tambahkan mapping untuk PANGKAT lainnya di sini
            }
        },
        {
            "Student Staf", new Dictionary<string, List<int>>()
            {
                { "3", new List<int> { 169, 170, 201 } },
                // Tambahkan mapping untuk PANGKAT lainnya di sini
            }
        }
    };
    }



    public class HeaderPenggajianMhs
    {
        public int ID_PENGGAJIAN { get; set; }
        public string NPP { get; set; }
        public string NAMA { get; set; }
        public string PANGKAT { get; set; }
        public string GOLONGAN { get; set; }
        public string JENJANG { get; set; }
        public string NPWP { get; set; }
        public string NO_TABUNGAN { get; set; }
        public string NAMA_BANK { get; set; }
        public string NAMA_REKENING { get; set; }
        public string NAMA_UNIT { get; set; }
        public string JENIS { get; set; }
    }

    public class SlipGajiAsistenViewModel
    {
        public HeaderPenggajianMhs Header { get; set; }
        public IEnumerable<DetailPenggajianMhsModel> Body { get; set; }
        public decimal TotalPenerimaanKotor { get; set; }
        public decimal TotalPajak { get; set; }
        public decimal TotalPenerimaanBersih { get; set; }
    }


}
