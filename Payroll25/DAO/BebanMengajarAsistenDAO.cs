using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class BebanMengajarAsistenDAO
    {
        public async Task<IEnumerable<BebanMengajarAsistenModel>> ShowBebanMengajarAsistenAsync(string NPMFilter = null, string NAMAFilter = null)
        {
            var connectionString = DBkoneksi.payrollkoneksi;

            if (string.IsNullOrEmpty(NPMFilter) && string.IsNullOrEmpty(NAMAFilter))
            {
                return new List<BebanMengajarAsistenModel>();
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    var query = @"SELECT 
                                TBL_VAKASI.ID_VAKASI,
                                TBL_ASISTEN.ID_ASISTEN,
                                mst_mhs_aktif.nama_mhs AS 'NAMA_ASISTEN', 
                                TBL_ASISTEN.NPM, 
                                REF_JENIS_ASISTEN.JENIS AS 'JENIS_ASISTEN', 
                                MST_KOMPONEN_GAJI.KOMPONEN_GAJI AS 'VAKASI', 
                                TBL_VAKASI.JUMLAH,
                                MST_TARIF_PAYROLL.NOMINAL AS 'TARIF'
                                FROM [PAYROLL].[payroll].[TBL_ASISTEN]
                                JOIN [PAYROLL].[dbo].[mst_mhs_aktif] ON [PAYROLL].[dbo].[mst_mhs_aktif].npm = [PAYROLL].[payroll].[TBL_ASISTEN].NPM
                                JOIN [PAYROLL].[payroll].[REF_JENIS_ASISTEN] ON REF_JENIS_ASISTEN.ID_JENIS_ASISTEN = TBL_ASISTEN.ID_JENIS_ASISTEN
                                JOIN [PAYROLL].[payroll].[TBL_VAKASI] ON TBL_VAKASI.NPP = TBL_ASISTEN.NPM
                                JOIN [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] ON MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI = TBL_VAKASI.ID_KOMPONEN_GAJI
                                JOIN[PAYROLL].[simka].[MST_TARIF_PAYROLL] ON MST_TARIF_PAYROLL.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI
                                where ID_REF_JENJANG IS NULL AND MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI = 77";

                    Dictionary<string, object> parameters = new Dictionary<string, object>();

                    if (!string.IsNullOrEmpty(NPMFilter))
                    {
                        query += " AND TBL_ASISTEN.NPM = @NPMFilter";
                        parameters.Add("@NPMFilter", NPMFilter);
                    }

                    if (!string.IsNullOrEmpty(NAMAFilter))
                    {
                        query += " AND mst_mhs_aktif.nama_mhs LIKE @NAMAFilter";
                        parameters.Add("@NAMAFilter", $"%{NAMAFilter}%");
                    }

                    return await conn.QueryAsync<BebanMengajarAsistenModel>(query, parameters);
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

        public int InsertBebanMengajarAsisten(BebanMengajarAsistenModel model)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"INSERT INTO [PAYROLL].[payroll].[TBL_VAKASI]
                                ([ID_KOMPONEN_GAJI],[ID_BULAN_GAJI],[NPP],[JUMLAH],[DATE_INSERTED],[DESKRIPSI])
                                VALUES
                                (@ID_KOMPONEN_GAJI,@ID_BULAN_GAJI,@NPP,@JUMLAH,@DATE_INSERTED,@DESKRIPSI)";

                    var parameters = new
                    {
                        ID_KOMPONEN_GAJI = 77,
                        ID_BULAN_GAJI = model.ID_BULAN_GAJI,
                        NPP = model.NPP,
                        JUMLAH = model.JUMLAH,
                        DATE_INSERTED = DateTime.Now, // Set to current time
                        DESKRIPSI = model.DESKRIPSI
                    };

                    return conn.Execute(query, parameters);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    throw;
                }
            }
        }

        public int UpdateBebanMengajarAsisten(List<BebanMengajarAsistenModel> model)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"UPDATE [payroll].[TBL_VAKASI]
                                SET 
                                [JUMLAH] = @JUMLAH
                                WHERE ID_VAKASI = @ID_VAKASI";

                    return conn.Execute(query, model);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    throw;
                }
            }
        }


        public int DeleteBebanMengajarAsisten(List<BebanMengajarAsistenModel> model)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"DELETE FROM [payroll].[TBL_VAKASI]
                                WHERE ID_VAKASI = @ID_VAKASI";

                    return conn.Execute(query, model);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"SQL Error: {sqlEx.Message}");
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<IEnumerable<TunjanganPengabdianModel>> GetBulanGaji(int tahun = 0)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"SELECT 
                                [ID_BULAN_GAJI]
                                FROM [PAYROLL].[payroll].[TBL_BULAN_GAJI]
                                WHERE [ID_TAHUN] = @inputTahun";

                    var parameters = new { inputTahun = tahun };

                    var data = await conn.QueryAsync<int>(query, parameters);

                    // Set nilai properti GET_BULAN_GAJI dengan data yang diperoleh dari database
                    var result = data.Select(id_bulan => new TunjanganPengabdianModel
                    {
                        GET_BULAN_GAJI = id_bulan
                    });

                    return result.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<TunjanganPengabdianModel>();
                }
            }
        }






    }
}
