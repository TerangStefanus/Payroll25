    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using Payroll25.Models;
    using static Payroll25.Models.BebanMengajarDosenModel;

    namespace Payroll25.DAO
    {
        public class BebanMengajarDosenDAO
        {
            public async Task<IEnumerable<BebanMengajarDosenModel>> ShowBebanMengajarAsync(string NPPFilter = null, int? TAHUNFilter = null, string NAMAFilter = null)
            {
                var connectionString = DBkoneksi.payrollkoneksi;

                // Jika keduanya tidak disediakan, kembalikan daftar kosong dengan segera
                if (string.IsNullOrEmpty(NPPFilter) && !TAHUNFilter.HasValue && string.IsNullOrEmpty(NAMAFilter))
                {
                    return new List<BebanMengajarDosenModel>();
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        await conn.OpenAsync();
                        var query = @"SELECT TOP 1000
                            BM.ID_BEBAN_MENGAJAR, 
                            K.NAMA, 
                            BM.NPP, 
                            BM.ID_TAHUN_AKADEMIK,
                            BM.NO_SEMESTER, 
                            BM.TOTAL_SKS,
                            TP.NAMA_TARIF_PAYROLL AS BEBAN_GELAR, 
                            TP.NOMINAL AS TARIF,
                            CONVERT(varchar, BM.TGL_AWAL_SK, 23) AS TGL_AWAL_SK,
                            CONVERT(varchar, BM.TGL_AKHIR_SK, 23) AS TGL_AKHIR_SK
                            FROM 
                                [PAYROLL].[payroll].[TBL_BEBAN_MENGAJAR] AS BM
                            JOIN 
                                [PAYROLL].[simka].[MST_KARYAWAN] AS K ON BM.NPP = K.NPP
                            JOIN 
                                [PAYROLL].[simka].[MST_TARIF_PAYROLL] AS TP ON K.ID_REF_JBTN_AKADEMIK = TP.ID_REF_JBTN_AKADEMIK 
                            JOIN
                                [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] AS KP ON TP.ID_KOMPONEN_GAJI = KP.ID_KOMPONEN_GAJI 
                            JOIN
                                [PAYROLL].[simka].[REF_JENJANG] AS RJ ON K.PENDIDIKAN_TERAKHIR = RJ.DESKRIPSI AND RJ.ID_REF_JENJANG = TP.ID_REF_JENJANG
                            WHERE KP.ID_KOMPONEN_GAJI = 77 ";

                        Dictionary<string, object> parameters = new Dictionary<string, object>();

                        if (!string.IsNullOrEmpty(NPPFilter))
                        {
                            query += " AND BM.NPP = @NPPFilter";
                            parameters.Add("@NPPFilter", NPPFilter);
                        }

                        if (TAHUNFilter.HasValue)
                        {
                            query += " AND BM.ID_TAHUN_AKADEMIK = @TAHUNFilter";
                            parameters.Add("@TAHUNFilter", TAHUNFilter);
                        }

                        if (!string.IsNullOrEmpty(NAMAFilter))
                        {
                            query += " AND K.NAMA LIKE @NAMAFilter";
                            parameters.Add("@NAMAFilter", $"%{NAMAFilter}%");
                        }

                        return await conn.QueryAsync<BebanMengajarDosenModel>(query, parameters);
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

        public int InsertBebanMengajar(BebanMengajarDosenModel model)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"INSERT INTO [PAYROLL].[payroll].[TBL_BEBAN_MENGAJAR]
                ([ID_TAHUN_AKADEMIK],[NO_SEMESTER],[NPP],[TOTAL_SKS],[BEBAN_STRUKTURAL],[KELEBIHAN_BEBAN],[TGL_AWAL_SK],[TGL_AKHIR_SK])
                VALUES
                (@ID_TAHUN_AKADEMIK,@NO_SEMESTER,@NPP,@TOTAL_SKS,@BEBAN_STRUKTURAL,@KELEBIHAN_BEBAN,@TGL_AWAL_SK,@TGL_AKHIR_SK)";

                    DateTime today = DateTime.Now;
                    DateTime awalSK, akhirSK;

                    // Check semester type
                    if (today.Month >= 9 && today.Month <= 12) // Gasal
                    {
                        awalSK = new DateTime(today.Year, 9, 25); // September 25th of the current year
                        akhirSK = new DateTime(today.Year + 1, 1, 25); // January 25th of the next year
                    }
                    else if (today.Month >= 2 && today.Month <= 7) // Genap
                    {
                        awalSK = new DateTime(today.Year, 2, 25); // February 25th of the current year
                        akhirSK = new DateTime(today.Year, 7, 25); // July 25th of the current year
                    }
                    else // Default case, handle as needed
                    {
                        // Add your default logic here
                        awalSK = DateTime.MinValue;
                        akhirSK = DateTime.MinValue;
                    }

                    var parameters = new
                    {
                        ID_TAHUN_AKADEMIK = model.ID_TAHUN_AKADEMIK,
                        NO_SEMESTER = model.NO_SEMESTER,
                        NPP = model.NPP,
                        TOTAL_SKS = model.TOTAL_SKS,
                        BEBAN_STRUKTURAL = (int?)null, // Set null
                        KELEBIHAN_BEBAN = (int?)null, // Set null
                        TGL_AWAL_SK = awalSK,
                        TGL_AKHIR_SK = akhirSK
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



        public int UpdateBebanMengajar(List<BebanMengajarDosenModel> model)
            {
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    try
                    {
                        var query = @"UPDATE [payroll].[TBL_BEBAN_MENGAJAR]
                                    SET 
                                    [TOTAL_SKS] = @TOTAL_SKS
                                    WHERE ID_BEBAN_MENGAJAR = @ID_BEBAN_MENGAJAR";

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

            // Buat Fungsi Delete 
            public int DeleteBebanMengajarDosen(List<BebanMengajarDosenModel> model)
            {
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    try
                    {
                        var query = @"DELETE FROM [payroll].[TBL_BEBAN_MENGAJAR]
                                    WHERE ID_BEBAN_MENGAJAR = @ID_BEBAN_MENGAJAR";

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




        }

    }
