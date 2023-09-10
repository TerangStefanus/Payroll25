using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payroll25.DAO
{
    public class KelolaTarifDAO
    {
        public async Task<IEnumerable<KelolaTarifModel>> ShowKelolaTarifAsync(string NAMAFilter = null)
        {
            var connectionString = DBkoneksi.payrollkoneksi;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    var query = @"SELECT TOP (76)
                                [ID_MST_TARIF_PAYROLL],
                                [ID_REF_JBTN_AKADEMIK],
                                [ID_REF_STRUKTURAL],
                                [ID_REF_GOLONGAN],
                                [ID_REF_FUNGSIONAL],
                                [ID_REF_JENJANG],
                                [NAMA_TARIF_PAYROLL],
                                [NOMINAL],
                                [JENIS],
                                [JENJANG_KELAS],
                                [ID_KOMPONEN_GAJI]
                                FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL]";

                    Dictionary<string, object> parameters = new Dictionary<string, object>();

                    if (!string.IsNullOrEmpty(NAMAFilter))
                    {
                        query += " WHERE NAMA_TARIF_PAYROLL LIKE @NAMAFilter";
                        parameters.Add("@NAMAFilter", $"%{NAMAFilter}%");
                    }

                    query += " ORDER BY [ID_MST_TARIF_PAYROLL] DESC";

                    return await conn.QueryAsync<KelolaTarifModel>(query, parameters);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    throw;
                }
            }
        }










    }

}
