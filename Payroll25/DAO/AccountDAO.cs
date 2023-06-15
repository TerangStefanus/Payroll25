using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class AccountDAO
    {
        public UserModel GetKaryawan (string npp)
        {
            using(SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    //k = Karyawan ur = USER_ROLE rr = REF_ROLE
                    string query = @"SELECT k.NPP, k.NAMA, k.PASSWORD,rr.DESKRIPSI
                                    FROM simka.MST_KARYAWAN a
                                    JOIN siatmax.TBL_USER_ROLE ur ON k.NPP = ur.NPP
                                    JOIN siatmax.REF_ROLE rr ON ur.ID_ROLE = rr.ID_ROLE
                                    WHERE k.NPP = @npp";

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


    }
}
