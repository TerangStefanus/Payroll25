using Dapper;
using Humanizer;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class TunjanganPengabdianDAO
    {
        public async Task<IEnumerable<TunjanganPengabdianModel>> ShowTunjanganPengabdianAsync(string NPPFilter = null, string NAMAFilter = null, string NPMFilter = null)
        {
            var connectionString = DBkoneksi.payrollkoneksi;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    string finalQuery = "";

                    

                    if (string.IsNullOrEmpty(NPPFilter) && string.IsNullOrEmpty(NAMAFilter) && string.IsNullOrEmpty(NPMFilter))
                    {
                        // Tidak ada filter, mengembalikan hasil kosong
                        return new List<TunjanganPengabdianModel>();
                    }

                    if (string.IsNullOrEmpty(NPPFilter) && string.IsNullOrEmpty(NAMAFilter) && string.IsNullOrEmpty(NPMFilter))
                    {
                        // Filter NPP dan NPM dan NAMA
                        finalQuery = KaryawanQuery(NPPFilter, NAMAFilter, null) + "UNION " + AsistenQuery(null, NAMAFilter, NPMFilter);
                    }
                    else if (!string.IsNullOrEmpty(NPPFilter) && !string.IsNullOrEmpty(NPMFilter))
                    {
                        // Filter NPP dan NPM
                        finalQuery = KaryawanQuery(NPPFilter, null, null) + "UNION " + AsistenQuery(null, null, NPMFilter);
                    }
                    else if (!string.IsNullOrEmpty(NPPFilter) && !string.IsNullOrEmpty(NAMAFilter))
                    {
                        // Filter NPP dan NAMA
                        finalQuery = KaryawanQuery(NPPFilter, NAMAFilter, null);
                    }
                    else if (!string.IsNullOrEmpty(NAMAFilter) && !string.IsNullOrEmpty(NPMFilter))
                    {
                        // Filter NPM dan NAMA
                        finalQuery = AsistenQuery(null, NAMAFilter, NPMFilter);
                    }
                    else if (!string.IsNullOrEmpty(NPPFilter))
                    {
                        // Filter NPP saja
                        finalQuery = KaryawanQuery(NPPFilter, null, null);
                    }
                    else if (!string.IsNullOrEmpty(NPMFilter))
                    {
                        // Filter NPM saja
                        finalQuery = AsistenQuery(null, null, NPMFilter);
                    }
                    else if (!string.IsNullOrEmpty(NAMAFilter))
                    {
                        // Filter NAMA saja
                        finalQuery = KaryawanQuery(null, NAMAFilter, null) + "UNION " + AsistenQuery(null, NAMAFilter, null);// Spasi sebelah union penting agar tidak terjadi query error
                    }
                    

                    return await conn.QueryAsync<TunjanganPengabdianModel>(finalQuery, GetParameters(NPPFilter, NAMAFilter, NPMFilter));
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


        private string KaryawanQuery(string NPPFilter, string NAMAFilter, string NPMFilter)
        {

            string conditionNPP = !string.IsNullOrEmpty(NPPFilter) ? "AND MST_KARYAWAN.NPP = @NPPFilter " : "";
            string conditionNAMA = !string.IsNullOrEmpty(NAMAFilter) ? " AND MST_KARYAWAN.NAMA LIKE @NAMAFilter " : "";

            if ( !string.IsNullOrEmpty(NPPFilter) || !string.IsNullOrEmpty(NAMAFilter) )
            {
                return $@"SELECT DISTINCT
                        TBL_VAKASI.ID_VAKASI,
                        TBL_VAKASI.NPP,
                        MST_KARYAWAN.NAMA,
                        MST_KOMPONEN_GAJI.KOMPONEN_GAJI,
                        TBL_VAKASI.JUMLAH,
                        MST_TARIF_PAYROLL.NOMINAL,
                        CONVERT(varchar, TBL_VAKASI.DATE_INSERTED, 101) AS TANGGAL
                        FROM PAYROLL.simka.MST_KARYAWAN
                        JOIN PAYROLL.payroll.TBL_VAKASI ON MST_KARYAWAN.NPP = TBL_VAKASI.NPP
                        JOIN PAYROLL.payroll.MST_KOMPONEN_GAJI ON TBL_VAKASI.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI
                        JOIN PAYROLL.simka.MST_TARIF_PAYROLL ON MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI = MST_TARIF_PAYROLL.ID_KOMPONEN_GAJI
                        WHERE MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI BETWEEN 198 AND 201
                        {conditionNPP}
                        {conditionNAMA}";            
            }

            return "";
        }


        private string AsistenQuery(string NPPFilter, string NAMAFilter, string NPMFilter)
        {
            string asistenQuery = " ";

            string conditionNPM = !string.IsNullOrEmpty(NPMFilter) ? "AND TBL_ASISTEN.NPM = @NPMFilter " : "";
            string conditionNAMA = !string.IsNullOrEmpty(NAMAFilter) ? " AND mst_mhs_aktif.nama_mhs LIKE @NAMAFilter " : "";


            if ( !string.IsNullOrEmpty(NAMAFilter) || !string.IsNullOrEmpty(NPMFilter) ) 
            {
                asistenQuery = $@"SELECT
                                TBL_VAKASI.ID_VAKASI,
                                TBL_VAKASI.NPP,
                                mst_mhs_aktif.nama_mhs AS 'NAMA',
                                MST_KOMPONEN_GAJI.KOMPONEN_GAJI, 
                                TBL_VAKASI.JUMLAH,
                                MST_TARIF_PAYROLL.NOMINAL,
                                CONVERT(varchar, TBL_VAKASI.DATE_INSERTED, 101) AS TANGGAL
                                FROM [PAYROLL].[payroll].[TBL_ASISTEN]
                                JOIN [PAYROLL].[dbo].[mst_mhs_aktif] ON [PAYROLL].[dbo].[mst_mhs_aktif].npm = [PAYROLL].[payroll].[TBL_ASISTEN].NPM
                                JOIN [PAYROLL].[payroll].[REF_JENIS_ASISTEN] ON REF_JENIS_ASISTEN.ID_JENIS_ASISTEN = TBL_ASISTEN.ID_JENIS_ASISTEN
                                JOIN [PAYROLL].[payroll].[TBL_VAKASI] ON TBL_VAKASI.NPP = TBL_ASISTEN.NPM
                                JOIN [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] ON MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI = TBL_VAKASI.ID_KOMPONEN_GAJI
                                JOIN [PAYROLL].[simka].[MST_TARIF_PAYROLL] ON MST_TARIF_PAYROLL.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI
                                WHERE ID_REF_JENJANG IS NULL AND MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI BETWEEN 198 AND 201
                                {conditionNPM}
                                {conditionNAMA}";
            }

            return asistenQuery;
        }


        private Dictionary<string, object> GetParameters(string NPPFilter, string NAMAFilter, string NPMFilter)
        {
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(NPPFilter))
            {
                parameters.Add("@NPPFilter", NPPFilter);
            }

            if (!string.IsNullOrEmpty(NAMAFilter))
            {
                parameters.Add("@NAMAFilter", $"%{NAMAFilter}%");
            }

            if (!string.IsNullOrEmpty(NPMFilter))
            {
                parameters.Add("@NPMFilter", NPMFilter);
            }

            return parameters;
        }




        public int InsertTunjanganPengabdian(TunjanganPengabdianModel model)
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

        public int UpdateTunjanganPengabdian(List<TunjanganPengabdianModel> model)
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

        public int DeleteTunjanganPengabdian(List<TunjanganPengabdianModel> model)
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

        // Support untuk Dropdown Insert Komponen Gaji
        public async Task<IEnumerable<TunjanganPengabdianModel>> GetKomponenGaji()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"SELECT 
                                [ID_KOMPONEN_GAJI],
                                [KOMPONEN_GAJI]
                                FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI]
                                WHERE MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI BETWEEN 198 AND 201 ";

                    var data = await conn.QueryAsync<TunjanganPengabdianModel>(query);

                    return data.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<TunjanganPengabdianModel>();
                }
            }
        }

        // Support untuk Dropdown Insert ID_BULAN_GAJI
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
