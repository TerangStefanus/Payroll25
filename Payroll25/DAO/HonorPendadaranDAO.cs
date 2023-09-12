using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class HonorPendadaranDAO
    {
 
        public async Task<IEnumerable<HonorPendadaranModel>> ShowHonorPendadaranAsync(string NPPFilter = null, string NAMAFilter = null)
        {
            var connectionString = DBkoneksi.payrollkoneksi;

            if (string.IsNullOrEmpty(NPPFilter) && string.IsNullOrEmpty(NAMAFilter))
            {
                return new List<HonorPendadaranModel>();
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    var query = @"SELECT
                                TBL_VAKASI.ID_VAKASI,
                                MST_KARYAWAN.NPP,
                                MST_KARYAWAN.NAMA,
                                MST_KOMPONEN_GAJI.KOMPONEN_GAJI,
                                TBL_VAKASI.JUMLAH,
                                MST_TARIF_PAYROLL.NOMINAL,
                                CONVERT(varchar, TBL_VAKASI.DATE_INSERTED, 101) AS TANGGAL
                                FROM 
                                    PAYROLL.simka.MST_KARYAWAN
                                JOIN 
                                    PAYROLL.payroll.TBL_VAKASI ON MST_KARYAWAN.NPP = TBL_VAKASI.NPP
                                JOIN 
                                    PAYROLL.payroll.MST_KOMPONEN_GAJI ON TBL_VAKASI.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI
                                JOIN
                                    PAYROLL.simka.MST_TARIF_PAYROLL ON MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI = MST_TARIF_PAYROLL.ID_KOMPONEN_GAJI
                                WHERE 
                                    MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI BETWEEN 140 AND 174";

                    Dictionary<string, object> parameters = new Dictionary<string, object>();

                    if (!string.IsNullOrEmpty(NPPFilter))
                    {
                        query += " AND MST_KARYAWAN.NPP = @NPPFilter";
                        parameters.Add("@NPPFilter", NPPFilter);
                    }

                    if (!string.IsNullOrEmpty(NAMAFilter))
                    {
                        query += " AND MST_KARYAWAN.NAMA LIKE @NAMAFilter";
                        parameters.Add("@NAMAFilter", $"%{NAMAFilter}%");
                    }

                    return await conn.QueryAsync<HonorPendadaranModel>(query, parameters);
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

        public int InsertHonorPendadaran(HonorPendadaranModel model)
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
                        ID_KOMPONEN_GAJI = model.ID_KOMPONEN_GAJI,
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

        public int UpdateHonorPendadaran(List<HonorPendadaranModel> model)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"UPDATE [payroll].[TBL_VAKASI]
                                SET
                                [NPP] = @NPP,
                                [ID_KOMPONEN_GAJI] = @ID_KOMPONEN_GAJI,
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










        // Support untuk Dropdown Insert Komponen Gaji
        public async Task<IEnumerable<HonorPendadaranModel>> GetKomponenGaji()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"SELECT 
                                [ID_KOMPONEN_GAJI],
                                [KOMPONEN_GAJI]
                                FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI]
                                WHERE ID_KOMPONEN_GAJI BETWEEN 140 AND 174;";

                    var data = await conn.QueryAsync<HonorPendadaranModel>(query);

                    return data.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<HonorPendadaranModel>();
                }
            }
        }

        // Support untuk Dropdown Insert ID_BULAN_GAJI
        public async Task<IEnumerable<HonorPendadaranModel>> GetBulanGaji(int tahun = 0)
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
                    var result = data.Select(id_bulan => new HonorPendadaranModel
                    {
                        GET_BULAN_GAJI = id_bulan
                    });

                    return result.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<HonorPendadaranModel>();
                }
            }
        }
    }
}
