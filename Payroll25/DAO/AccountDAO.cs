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

        //ambil dari DB payroll.TBL_ASISTEN

        //Masukkan dulu nama mahasiswa Student Staff / Assisten Lab yang bisa login

        public bool CheckAsisten(string npm)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    string query = @"SELECT COUNT(*) 
                             FROM [PAYROLL].[payroll].[TBL_ASISTEN]
                             WHERE NPM = @npm";

                    var param = new { npm = npm };
                    var count = conn.ExecuteScalar<int>(query, param);

                    return count > 0;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        

        public MhsModel GetMahasiswa(string npm)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.siatmakoneksi))
            {
                try
                {
                    //k = Karyawan ur = USER_ROLE rr = REF_ROLE
                    string query = @"SELECT 
                                    a.NPM,
                                    a.PASSWORD, 
                                    'Mahasiswa' AS ROLE,
                                    a.NAMA_MHS, 
                                    a.TGL_LAHIR,
                                    a.ALAMAT,
                                    a.NO_HP
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


        // Ambil dari DB MST_KARYAWAN Kontrak

        public DosenKontrakModel GetDosenKontrak(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    string query = @"SELECT 
                            [NPP],
                            [PASSWORD_RIPEM],
                            'Dosen Kontrak' AS ROLE,
                            [NAMA],
                            [TGL_LAHIR],
                            [ALAMAT],
                            [NO_TELPON_HP]
                            FROM [PAYROLL].[simka].[MST_KARYAWAN]
                            WHERE 
                            simka.MST_KARYAWAN.STATUS_KEPEGAWAIAN = 'Kontrak' 
                            and [simka].[MST_KARYAWAN].[NPP] = @username";

                    var param = new { username = npp };
                    var data = conn.QueryFirstOrDefault<DosenKontrakModel>(query, param);

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



    }
}
