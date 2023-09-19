using Microsoft.AspNetCore.Mvc.Rendering;

namespace Payroll25.Models
{
    public class DataGajiModel
    {
        // Mencari Bulan & Tahun
        public int ID_BULAN { get; set; }
        public string BULAN { get; set; }
        public int TAHUN { get; set; }
        // 

        // Mencari Unit 
        public int ID_UNIT { get; set; }
        public string NAMA_UNIT { get; set; }
        //

        // Mencari Data Karyawan 

        public int ID_PENGGAJIAN { get; set; }
        public string NPP { get; set; }
        public string NAMA { get; set; }
        public string GOLONGAN { get; set; }
        public string JENJANG { get; set; }
        public string NPWP { get; set; }
        public string NO_REKENING { get; set; }
        public string NAMA_BANK { get; set; }
        public string NAMA_REKENING { get; set; }
        //

        public List<DetailGaji> DetailGaji { get; set; }

        public class DataGajiViewModel
        {
            public IEnumerable<DataGajiModel> DataGajiList { get; set; }
            public DataGajiModel DataGaji { get; set; }
        }
    }

    public class DetailGaji
    {
        public string KOMPONEN_GAJI { get; set; }
        public float JUMLAH_SATUAN { get; set; }
        public float NOMINAL { get; set; }
    }

    


}
