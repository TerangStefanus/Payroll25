using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class IdentitasDosenDAO
    {
        public IEnumerable<IdentitasDosenModel> ShowIdentitasDosen()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var parameters = new DynamicParameters();
                    // Jika Anda memiliki parameter dalam query, Anda dapat menambahkannya ke objek DynamicParameters.
                    // parameters.Add("parameterName", parameterValue);

                    var query = @"SELECT TOP 50 NPP, NAMA, NAMA_LENGKAP_GELAR, 
                                  TGL_LAHIR, JNS_KEL, AGAMA, ID_UNIT, ID_UNIT_AKADEMIK, ID_REF_GOLONGAN, 
                                  ID_REF_JBTN_AKADEMIK, NO_TELPON_HP, NPWP, ALAMAT, USERNAME, PASSWORD, PASSWORD_RIPEM 
                                  FROM simka.MST_KARYAWAN ";

                    var data = conn.Query<IdentitasDosenModel>(query, parameters).ToList();

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
