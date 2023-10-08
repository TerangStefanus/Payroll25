namespace Payroll25.Models
{
    public class UserModel
    {
        public string npp { get; set; }
        public string nama { get; set; }
        public string password { get; set; }
        public string deskripsi { get; set; }
        public string password_ripem { get; set; }
    }

    public class ModelMenu
    {
        public int ID_SI_MENU { get; set; }
        public string DESKRIPSI { get; set; }
        public string LINK { get; set; }
    }

    public class ModelSubMenu
    {
        public int ID_SI_SUBMENU { get; set; }
        public int ID_SI_MENU { get; set; }
        public string DESKRIPSI { get; set; }
        public string LINK { get; set; }
    }

    public class MhsModel
    {
        public string NPM { get; set; }
        public string NAMA_MHS { get; set; }
        public string PASSWORD { get; set; }    
        public string ROLE { get; set; }  
        public string NO_HP { get; set; }
        public string TGL_LAHIR { get; set;}
        public string ALAMAT { get; set; }
    }

    public class DosenKontrakModel
    {
        public string NPP { get; set; }
        public string PASSWORD_RIPEM { get; set; }
        public string ROLE { get; set; }
        public string NAMA { get; set; }
        public DateTime TGL_LAHIR { get; set; }
        public string ALAMAT { get; set; }
        public string NO_TELPON_HP { get; set; }
    }

    public class PelatihModel
    {
        public string NPP { get; set; }
        public string PASSWORD { get; set; }
        public string ROLE { get; set; }
        public string NAMA { get; set; }
        public DateTime TGL_LAHIR { get; set; }
        public string ALAMAT { get; set; }
        public string NO_TELP { get; set; }
    }




}
