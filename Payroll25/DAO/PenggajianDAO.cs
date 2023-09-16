using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class PenggajianDAO
    {

        public async Task<IEnumerable<PenggajianModel>> GetPenggajianDataAsync(string NPPFilter = null)
        {
            var connectionString = DBkoneksi.payrollkoneksi;

            if (string.IsNullOrEmpty(NPPFilter))
            {
                return new List<PenggajianModel>();
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = @"
                    DECLARE @NPP VARCHAR(50);
                    SET @NPP = @NppParam;

                    SELECT 
                        TBL_BEBAN_MENGAJAR.NPP, 
                        MST_KOMPONEN_GAJI.KOMPONEN_GAJI AS NAMA_KOMPONEN_GAJI, 
                        TBL_BEBAN_MENGAJAR.TOTAL_SKS AS JUMLAH, 
                        MST_TARIF_PAYROLL.NOMINAL AS TARIF
                    FROM 
                        [PAYROLL].[payroll].[TBL_BEBAN_MENGAJAR]
                    INNER JOIN 
                        [PAYROLL].[simka].[MST_KARYAWAN] ON TBL_BEBAN_MENGAJAR.NPP = MST_KARYAWAN.NPP
                    JOIN 
                        [PAYROLL].[simka].[MST_TARIF_PAYROLL] ON MST_KARYAWAN.ID_REF_JBTN_AKADEMIK = MST_TARIF_PAYROLL.ID_REF_JBTN_AKADEMIK 
                    JOIN
                        [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] ON MST_TARIF_PAYROLL.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI 
                    JOIN
                        [PAYROLL].[simka].[REF_JENJANG] ON MST_KARYAWAN.PENDIDIKAN_TERAKHIR = REF_JENJANG.DESKRIPSI AND REF_JENJANG.ID_REF_JENJANG = MST_TARIF_PAYROLL.ID_REF_JENJANG
                    WHERE 
                        MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI = 77
                        AND TBL_BEBAN_MENGAJAR.NPP = @NPP

                    UNION ALL 

                    SELECT DISTINCT
                        MST_KARYAWAN.NPP, 
                        MST_KOMPONEN_GAJI.KOMPONEN_GAJI AS NAMA_KOMPONEN_GAJI, 
                        TBL_VAKASI.JUMLAH, 
                        MST_TARIF_PAYROLL.NOMINAL AS TARIF
                    FROM 
                        PAYROLL.simka.MST_KARYAWAN
                    JOIN 
                        PAYROLL.payroll.TBL_VAKASI ON MST_KARYAWAN.NPP = TBL_VAKASI.NPP
                    JOIN 
                        PAYROLL.payroll.MST_KOMPONEN_GAJI ON TBL_VAKASI.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI
                    JOIN
                        PAYROLL.simka.MST_TARIF_PAYROLL ON MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI = MST_TARIF_PAYROLL.ID_KOMPONEN_GAJI
                    WHERE 
                        MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI BETWEEN 78 AND 201 
                        AND MST_KARYAWAN.NPP = @NPP";

                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@NppParam", NPPFilter }
                    };

                    return await conn.QueryAsync<PenggajianModel>(query, parameters);
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
