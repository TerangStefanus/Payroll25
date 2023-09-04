using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class IdentitasPelatihDAO
    {
        public IEnumerable<IdentitasPelatihModel> ShowIdentitasPelatih()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var parameters = new DynamicParameters();

                    var query = @"SELECT
                                [PAYROLL].[payroll].[TBL_PELATIH].[ID_PELATIH],
                                [PAYROLL].[payroll].[TBL_PELATIH].[NPP],
                                [PAYROLL].[payroll].[TBL_PELATIH].[NAMA],
                                [PAYROLL].[payroll].[TBL_PELATIH].[ID_TAHUN_AKADEMIK],
                                [PAYROLL].[payroll].[TBL_PELATIH].[NO_SEMESTER],
	                            [PAYROLL].[siatmax].[MST_UNIT].[ID_UNIT],
	                            [PAYROLL].[siatmax].[MST_UNIT].[NAMA_UNIT],
                                [PAYROLL].[payroll].[TBL_PELATIH].[NO_REKENING],
                                [PAYROLL].[payroll].[TBL_PELATIH].[NAMA_REKENING],
                                [PAYROLL].[payroll].[TBL_PELATIH].[NAMA_BANK]
                                FROM [PAYROLL].[payroll].[TBL_PELATIH]
                                INNER JOIN [PAYROLL].[siatmax].[MST_UNIT] ON [PAYROLL].[payroll].[TBL_PELATIH].[ID_UNIT] = [PAYROLL].[siatmax].[MST_UNIT].[ID_UNIT]";

                    var data = conn.Query<IdentitasPelatihModel>(query, parameters).ToList();

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
