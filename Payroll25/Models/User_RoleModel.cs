namespace Payroll25.Models
{
    public class User_RoleModel
    {
        public int ID_SISTEM_INFORMASI { get; set; }
        public int ID_ROLE { get; set; }
        public string NPP { get; set; }
        public DateTime TGL_AWAL_AKTIF { get; set; }
        public DateTime TGL_AKHIR_AKTIF { get; set; }
        public int ID_FAKULTAS { get; set; }    
    }
}
