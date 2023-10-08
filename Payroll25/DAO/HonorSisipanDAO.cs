using CsvHelper.Configuration;
using CsvHelper;
using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;
using System.Globalization;

namespace Payroll25.DAO
{
    public class HonorSisipanDAO
    {
        public async Task<IEnumerable<HonorSisipanModel>> ShowHonorSisipan(string NPPFilter = null)
        {
            var connectionString = DBkoneksi.payrollkoneksi;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string topClause = string.IsNullOrEmpty(NPPFilter) ? "TOP 300" : "";
                    string whereClause = string.IsNullOrEmpty(NPPFilter) ? "" : "WHERE NPP = @NPPFilter";

                    var parameters = new Dictionary<string, object>();

                    if (!string.IsNullOrEmpty(NPPFilter))
                    {
                        parameters.Add("@NPPFilter", NPPFilter);
                    }

                    string query = $@"SELECT {topClause} 
                                    TBL_VAKASI.[ID_VAKASI],
                                    TBL_VAKASI.[ID_KOMPONEN_GAJI],
                                    MST_KOMPONEN_GAJI.KOMPONEN_GAJI,
                                    TBL_VAKASI.[ID_BULAN_GAJI],
                                    TBL_VAKASI.[NPP],
                                    TBL_VAKASI.[JUMLAH],
                                    TBL_VAKASI.[DATE_INSERTED],
                                    TBL_VAKASI.[DESKRIPSI]
                                    FROM [PAYROLL].[payroll].[TBL_VAKASI]
                                    JOIN PAYROLL.payroll.MST_KOMPONEN_GAJI ON TBL_VAKASI.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI
                                    {whereClause}
                                    ORDER BY ID_BULAN_GAJI DESC";

                    return await conn.QueryAsync<HonorSisipanModel>(query, parameters);
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

        public (bool, List<string>) UploadAndInsertCSV(IFormFile csvFile)
        {
            var errorMessages = new List<string>();
            try
            {
                using (var stream = csvFile.OpenReadStream())
                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    conn.Open();
                    var transaction = conn.BeginTransaction();

                    try
                    {
                        var records = csv.GetRecords<HonorSisipanModel>().ToList();
                        var validRecords = records.Where(record =>
                            record.ID_KOMPONEN_GAJI != null &&
                            record.ID_BULAN_GAJI != null &&
                            !string.IsNullOrEmpty(record.NPP) &&
                            record.JUMLAH != null 
                        ).ToList();

                        var invalidRecords = records.Except(validRecords).ToList();

                        foreach (var invalidRecord in invalidRecords)
                        {
                            errorMessages.Add($"Record dengan NPP {invalidRecord.NPP} memiliki data yang tidak valid atau tidak lengkap.");
                        }

                        foreach (var record in validRecords)
                        {
                            var insertQuery = @"INSERT INTO [payroll].[TBL_VAKASI] 
                                                (ID_KOMPONEN_GAJI, ID_BULAN_GAJI, NPP, JUMLAH) 
                                                VALUES 
                                                (@ID_KOMPONEN_GAJI, @ID_BULAN_GAJI, @NPP, @JUMLAH)";

                            conn.Execute(insertQuery, record, transaction);
                        }

                        transaction.Commit();
                        return (true, errorMessages);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error: " + ex.Message);
                        errorMessages.Add(ex.Message);
                        return (false, errorMessages);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                errorMessages.Add(ex.Message);
                return (false, errorMessages);
            }
        }

        public int UpdateHonorSisipan(List<HonorSisipanModel> model)
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

        public int DeleteHonorSisipan(List<HonorSisipanModel> model)
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

        // Support untuk Dropdown Komponen Gaji
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
                                WHERE MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI BETWEEN 140 AND 174 
                                OR 
                                MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI BETWEEN 190 AND 197";

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

        // Support untuk Dropdown ID_BULAN_GAJI
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
