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

}
