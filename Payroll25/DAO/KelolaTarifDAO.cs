    using Dapper;
    using Payroll25.Models;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;

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
                    var query = @"SELECT 
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
                                    MST_KOMPONEN_GAJI.[ID_KOMPONEN_GAJI]
                                    FROM 
                                    [PAYROLL].[simka].[MST_TARIF_PAYROLL]
                                    JOIN 
                                    [PAYROLL].[MST_KOMPONEN_GAJI] ON [PAYROLL].[simka].[MST_TARIF_PAYROLL].[ID_KOMPONEN_GAJI] = [PAYROLL].[MST_KOMPONEN_GAJI].[ID_KOMPONEN_GAJI]
                                    WHERE 
                                    [PAYROLL].[MST_KOMPONEN_GAJI].[JENIS_FUNGSIONAL] = 'Kontrak'
                                    AND
                                    [PAYROLL].[MST_KOMPONEN_GAJI].IS_DELETED = 0
                                    AND
                                    [PAYROLL].[simka].[MST_TARIF_PAYROLL].ISACTIVE = 1";
             


                    Dictionary<string, object> parameters = new Dictionary<string, object>();

                        if (!string.IsNullOrEmpty(NAMAFilter))
                        {
                            query += " AND NAMA_TARIF_PAYROLL LIKE @NAMAFilter";
                            parameters.Add("@NAMAFilter", $"%{NAMAFilter}%");
                        }

                        query += " ORDER BY [ID_KOMPONEN_GAJI] DESC";

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


        public int InsertKelolaTarif(KelolaTarifModel model)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Insert komponen gaji ke database
                    var komponenGajiQuery = @"INSERT INTO PAYROLL.payroll.MST_KOMPONEN_GAJI
                                            ([ID_JNS_KOMPONEN],[KOMPONEN_GAJI],[NO_URUT],[IS_SATUAN],[JENIS_FUNGSIONAL],[IS_DELETED])
                                            VALUES
                                            (@ID_JNS_KOMPONEN,@KOMPONEN_GAJI,@NO_URUT,@IS_SATUAN,@JENIS_FUNGSIONAL,@IS_DELETED)";

                    var komponenGajiParameters = new
                    {
                        ID_JNS_KOMPONEN = 1,
                        KOMPONEN_GAJI = model.KOMPONEN_GAJI, // Menggunakan KOMPONEN_GAJI dari model
                        NO_URUT = 1,
                        IS_SATUAN = 0,
                        JENIS_FUNGSIONAL = "Kontrak", // Ubah tipe data ke string
                        IS_DELETED = 0
                    };

                    conn.Execute(komponenGajiQuery, komponenGajiParameters, transaction);

                    // 2. Cari ID_KOMPONEN_GAJI berdasarkan NAMA_TARIF_PAYROLL
                    var searchQuery = "SELECT ID_KOMPONEN_GAJI FROM PAYROLL.payroll.MST_KOMPONEN_GAJI WHERE KOMPONEN_GAJI = @KOMPONEN_GAJI";
                    var searchParameters = new { KOMPONEN_GAJI = model.KOMPONEN_GAJI };
                    var searchResult = conn.QueryFirstOrDefault(searchQuery, searchParameters, transaction);

                    if (searchResult == null)
                    {
                        throw new Exception("Data komponen gaji tidak ditemukan");
                    }

                    // 3. Insert tarif berdasarkan nama komponen dan id komponen gaji
                    var tarifQuery = @"INSERT INTO PAYROLL.[simka].[MST_TARIF_PAYROLL]
                                    ([NAMA_TARIF_PAYROLL],[NOMINAL],[ID_KOMPONEN_GAJI],[JENIS],[ISACTIVE],[JENJANG_KELAS])
                                    VALUES
                                    (@NAMA_TARIF_PAYROLL,@NOMINAL,@ID_KOMPONEN_GAJI,@JENIS,@ISACTIVE,@JENJANG_KELAS)";

                    var tarifParameters = new
                    {
                        NAMA_TARIF_PAYROLL = model.KOMPONEN_GAJI,
                        NOMINAL = model.NOMINAL,
                        ID_KOMPONEN_GAJI = searchResult.ID_KOMPONEN_GAJI,// Menggunakan ID dari hasil pencarian
                        JENIS = "Komponen Gaji",
                        ISACTIVE = 1,
                        JENJANG_KELAS = "Tidak Diketahui"
                    };

                    conn.Execute(tarifQuery, tarifParameters, transaction);

                    // Commit transaksi jika semua operasi berhasil
                    transaction.Commit();

                    return 1; // Atau nilai yang sesuai dengan kebutuhan Anda
                }
                catch (SqlException sqlEx)
                {
                    // Rollback transaksi jika terjadi kesalahan
                    transaction.Rollback();
                    // Handle atau lempar kembali exception jika diperlukan
                    throw;
                }
            }
        }



        public int UpdateKelolaTarif(List<KelolaTarifModel> model)
            {
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    try
                    {
                        var query = @"UPDATE [PAYROLL].[simka].[MST_TARIF_PAYROLL]
                                    SET 
                                    [NOMINAL] = @NOMINAL
                                    WHERE ID_MST_TARIF_PAYROLL = @ID_MST_TARIF_PAYROLL";

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

            public async Task<IEnumerable<KelolaTarifModel>> GetKomponenGaji()
            {
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    try
                    {
                        var query = @"SELECT 
                                    [ID_KOMPONEN_GAJI],
                                    [KOMPONEN_GAJI]
                                    FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI]
                                    WHERE JENIS_FUNGSIONAL = 'Kontrak'";

                        var data = await conn.QueryAsync<KelolaTarifModel>(query);

                        return data.ToList();
                    }
                    catch (Exception)
                    {
                        // Handle exceptions here
                        return Enumerable.Empty<KelolaTarifModel>();
                    }
                }
            }


        // Fungsi ini tidak benar benar menghapus tarif tetapi hanya mengubah status is delete dan is activenya saja
        public int DeleteKelolaTarif(List<KelolaTarifModel> model)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                conn.Open();

                try
                {
                    foreach (var tarif in model)
                    {
                        // Step 1: Find ID_KOMPONEN_GAJI in MST_TARIF_PAYROLL
                        var selectQuery = @"SELECT ID_KOMPONEN_GAJI FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] WHERE ID_MST_TARIF_PAYROLL = @ID_MST_TARIF_PAYROLL";
                        int idKomponenGaji = conn.ExecuteScalar<int>(selectQuery, tarif);

                        // Logging for Step 2
                        if (idKomponenGaji != 0)
                        {
                            Console.WriteLine($"Found ID_KOMPONEN_GAJI: {idKomponenGaji}");
                        }
                        else
                        {
                            Console.WriteLine($"ID_KOMPONEN_GAJI not found for ID_MST_TARIF_PAYROLL: {tarif.ID_MST_TARIF_PAYROLL}");
                        }

                        // Step 2: Update tarif with ID_MST_TARIF_PAYROLL 
                        var updateTarifQuery = @"UPDATE [simka].[MST_TARIF_PAYROLL] SET [ISACTIVE] = 0 WHERE ID_MST_TARIF_PAYROLL = @ID_MST_TARIF_PAYROLL";
                        Console.WriteLine($"Updating tarif with ID_MST_TARIF_PAYROLL: {tarif.ID_MST_TARIF_PAYROLL}");
                        conn.Execute(updateTarifQuery, tarif);

                        // Step 3: Update MST_KOMPONEN_GAJI with ID_KOMPONEN_GAJI
                        var updateKomponenQuery = @"UPDATE [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] SET [IS_DELETED] = 1 WHERE ID_KOMPONEN_GAJI = @ID_KOMPONEN_GAJI";
                        Console.WriteLine($"Updating MST_KOMPONEN_GAJI with ID_KOMPONEN_GAJI: {idKomponenGaji}");
                        conn.Execute(updateKomponenQuery, new { ID_KOMPONEN_GAJI = idKomponenGaji });
                    }

                    return model.Count; // Return the number of tarifs deleted
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









    }

}

    
