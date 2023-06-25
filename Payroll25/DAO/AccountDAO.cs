using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class AccountDAO
    {
        public UserModel GetKaryawan(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    //k = Karyawan ur = USER_ROLE rr = REF_ROLE
                    string query = @"SELECT k.NPP, k.NAMA, k.PASSWORD,k.PASSWORD_RIPEM,rr.DESKRIPSI
                                    FROM simka.MST_KARYAWAN k
                                    JOIN siatmax.TBL_USER_ROLE ur ON k.NPP = ur.NPP
                                    JOIN siatmax.REF_ROLE rr ON ur.ID_ROLE = rr.ID_ROLE
                                    WHERE k.NPP = @username";

                    var param = new { username = npp };
                    var data = conn.QueryFirstOrDefault<UserModel>(query, param);

                    return data;
                }
                catch (Exception)
                {

                    return null;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public List<ModelMenu> GetMenuKaryawan(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    // si = sistem_informasi sm = si_menu ssm = si_submenu rsm = role_submenu
                    // k = Karyawan ur = USER_ROLE rr = REF_ROLE
                    string query = @"SELECT DISTINCT sm.*
                                    FROM siatmax.TBL_SISTEM_INFORMASI si
                                    JOIN siatmax.TBL_SI_MENU sm ON si.ID_SISTEM_INFORMASI = sm.ID_SISTEM_INFORMASI
                                    JOIN siatmax.TBL_SI_SUBMENU ssm ON sm.ID_SI_MENU = ssm.ID_SI_MENU
                                    JOIN siatmax.TBL_ROLE_SUBMENU rsm ON rsm.ID_SI_SUBMENU = ssm.ID_SI_SUBMENU
                                    JOIN siatmax.REF_ROLE rr ON rr.ID_ROLE = rsm.ID_ROLE
                                    JOIN siatmax.TBL_USER_ROLE ur ON ur.ID_ROLE = rr.ID_ROLE
                                    JOIN simka.MST_KARYAWAN k ON k.NPP = ur.NPP
                                    WHERE rr.DESKRIPSI =@role";

                    var param = new { role = npp };
                    var data = conn.Query<ModelMenu>(query, param).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public List<ModelSubMenu> GetSubMenuKaryawan(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    // si = sistem_informasi sm = si_menu ssm = si_submenu rsm = role_submenu
                    // k = Karyawan ur = USER_ROLE rr = REF_ROLE
                    string query = @"SELECT DISTINCT ssm.* 
                                    FROM siatmax.TBL_ROLE_SUBMENU rsm
                                    JOIN siatmax.REF_ROLE rr ON rr.ID_ROLE = rsm.ID_ROLE
                                    JOIN siatmax.TBL_SI_SUBMENU ssm ON rsm.ID_SI_SUBMENU = ssm.ID_SI_SUBMENU
                                    JOIN siatmax.TBL_SI_MENU sm ON sm.ID_SI_MENU = sm.ID_SI_MENU
                                    WHERE rr.DESKRIPSI = @role AND ssm.ISACTIVE = 1 AND sm.ISACTIVE = 1";

                    var param = new { role = npp };
                    var data = conn.Query<ModelSubMenu>(query, param).AsList();

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        //public getMahasiswaAssisten

        //ambil dari DB SIATMA_UAJY ( Semua mahasiswa )
        //filter nama - npm kemudian dari prodi/fakultas

        public MhsModel GetMahasiswa(string npm)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.siatmakoneksi))
            {
                try
                {
                    //k = Karyawan ur = USER_ROLE rr = REF_ROLE
                    string query = @"SELECT a.NPM, a.PASSWORD, 'Mahasiswa' AS ROLE, a.NAMA_MHS, a.TGL_LAHIR,a.ALAMAT,a.NO_HP
                                    FROM MST_MHS_AKTIF a
                                    WHERE a.NPM = @username";

                    var param = new { username = npm };
                    var data = conn.QueryFirstOrDefault<MhsModel>(query, param);

                    return data;
                }
                catch (Exception)
                {

                    return null;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        //ambil dari DB payroll.TBL_ASISTEN
        //Masukkan dulu nama mahasiswa Student Staff / Assisten Lab yang bisa login

    }
}
