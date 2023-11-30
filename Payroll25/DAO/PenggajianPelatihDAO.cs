using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;
using static Payroll25.Models.PenggajianPelatihModel;

namespace Payroll25.DAO
{
    public class PenggajianPelatihDAO
    {
        public async Task<IEnumerable<PenggajianModel>> GetBulanGaji(int tahun = 0)
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
                    var result = data.Select(id_bulan => new PenggajianModel
                    {
                        GET_BULAN_GAJI = id_bulan
                    });

                    return result.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<PenggajianModel>();
                }
            }
        }

        public async Task<List<string>> GetUnits()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    await conn.OpenAsync();
                    const string query = @"SELECT [NAMA_UNIT] FROM [PAYROLL].[siatmax].[MST_UNIT]";
                    var result = await conn.QueryAsync<string>(query);
                    return result.ToList();
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

        public async Task<List<PelatihDataModel>> GetPelatihData(string unit = null)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = @"SELECT DISTINCT
                            [ID_PELATIH],
                            [TBL_PELATIH].[NPP],
                            [NAMA],
                            [ID_TAHUN_AKADEMIK],
                            [NO_SEMESTER],
                            [TBL_PELATIH].[ID_UNIT],
                            [NO_REKENING],
                            [NAMA_REKENING],
                            [NAMA_BANK]
                            FROM [PAYROLL].[payroll].[TBL_PELATIH]
                            JOIN [PAYROLL].[siatmax].[MST_UNIT] ON [MST_UNIT].ID_UNIT = [TBL_PELATIH].ID_UNIT";

                    if (!string.IsNullOrEmpty(unit))
                    {
                        query += " WHERE MST_UNIT.NAMA_UNIT = @Unit";
                    }

                    query += " ORDER BY [TBL_PELATIH].ID_PELATIH DESC";

                    var parameters = new { Unit = unit };

                    var result = await conn.QueryAsync<PelatihDataModel>(query, parameters);

                    return result.ToList();
                }
                catch (Exception ex)
                {
                    // Handle exception here, e.g., log it, rethrow it, etc.
                    return null;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public async Task<bool> IsDataExist(string npp, int idBulanGaji)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    string query = @"SELECT COUNT(*) 
                                    FROM [PAYROLL].[payroll].[TBL_PENGGAJIAN] 
                                    WHERE NPP = @NPP AND ID_BULAN_GAJI = @ID_BULAN_GAJI";

                    var count = await conn.ExecuteScalarAsync<int>(query, new { NPP = npp, ID_BULAN_GAJI = idBulanGaji });

                    return count > 0;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public async Task<bool> InsertToTblPenggajianPelatih(PenggajianPelatihModel insertData)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    string query = @"INSERT INTO [PAYROLL].[payroll].[TBL_PENGGAJIAN]
                                    ([NPP],[ID_BULAN_GAJI],[NAMA],[STATUS_KEPEGAWAIAN],[MASA_KERJA_RIIL],[MASA_KERJA_GOL],[JBT_STRUKTURAL],
                                    [JBT_AKADEMIK],[JBT_FUNGSIONAL],[PANGKAT],[GOLONGAN],[JENJANG],[NO_TABUNGAN],[NPWP])
                                    VALUES
                                    (@NPP,@ID_BULAN_GAJI,@NAMA,@STATUS_KEPEGAWAIAN,@MASA_KERJA_RIIL,@MASA_KERJA_GOL,@JBT_STRUKTURAL,
                                    @JBT_AKADEMIK,@JBT_FUNGSIONAL,@PANGKAT,@GOLONGAN,@JENJANG,@NO_TABUNGAN,@NPWP)";

                    var result = await conn.ExecuteAsync(query, insertData);

                    return result > 0;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public async Task<bool> AutoInsertPenggajianPelatih(int idBulanGaji, string tahun, string unit)
        {
            try
            {
                var pelatihData = await GetPelatihData(unit); // 1. Get Data Pelatih

                foreach (var pelatih in pelatihData) // 2. Lakukan secara iterasi berdasarkan NPP Pelatih
                {
                    if (!await IsDataExist(pelatih.NPP, idBulanGaji)) // 3. Cek Apakah data Penggajian dengan ID_BULAN_GAJI dan NPP sudah ada 
                    {

                        var insertData = new PenggajianPelatihModel
                        {
                            NPP = pelatih.NPP,
                            ID_BULAN_GAJI = idBulanGaji,
                            NAMA = pelatih.NAMA,
                            STATUS_KEPEGAWAIAN = "Kontrak",
                            MASA_KERJA_RIIL = "0",
                            MASA_KERJA_GOL = "0",
                            JBT_STRUKTURAL = "-",
                            JBT_AKADEMIK = "-",
                            JBT_FUNGSIONAL = "-",
                            PANGKAT = null,
                            GOLONGAN = "II/A",
                            JENJANG = null, // Gunakan variabel jenjang yang sudah ditentukan
                            NO_TABUNGAN = pelatih.NO_REKENING,
                            NPWP = null
                        };

                        await InsertToTblPenggajianPelatih(insertData); // 4. Insert Data ke TBL_PENGGAJIAN 
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Auto Hitung Gaji 

        public async Task<IEnumerable<PenggajianPelatihModel>> GetPenggajianDataPelatih(int idBulanGaji, string unit = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    await conn.OpenAsync();

                    var query = @"SELECT 
                        [ID_PENGGAJIAN], 
                        [payroll].[TBL_PENGGAJIAN].[NPP], 
                        [ID_BULAN_GAJI], 
                        [TBL_PENGGAJIAN].[NAMA], 
                        [STATUS_KEPEGAWAIAN], 
                        [PANGKAT], 
                        [GOLONGAN], 
                        [JENJANG], 
                        [NO_TABUNGAN], 
                        [NPWP]
                        FROM [PAYROLL].[payroll].[TBL_PENGGAJIAN]
                        JOIN [PAYROLL].[payroll].[TBL_PELATIH] ON [TBL_PENGGAJIAN].NPP = [TBL_PELATIH].NPP
                        JOIN [PAYROLL].[siatmax].[MST_UNIT] ON [MST_UNIT].ID_UNIT = [TBL_PELATIH].ID_UNIT
                        WHERE [TBL_PENGGAJIAN].[ID_BULAN_GAJI] = @IdBulanGaji";

                    if (!string.IsNullOrEmpty(unit))
                    {
                        query += " AND [MST_UNIT].NAMA_UNIT = @Unit";
                    }

                    query += " ORDER BY [TBL_PENGGAJIAN].ID_PENGGAJIAN DESC";

                    var parameters = new { IdBulanGaji = idBulanGaji, Unit = unit };

                    var result = await conn.QueryAsync<PenggajianPelatihModel>(query, parameters);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                return null;
            }
        }


        public async Task<IEnumerable<KomponenGajiPelatihModel>> GetKomponenGajiMhs(int idBulanGaji, string NPP)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    await conn.OpenAsync();

                    var query = @"SELECT
                                [ID_BULAN_GAJI],
                                [NPP],
                                [TBL_VAKASI].[ID_KOMPONEN_GAJI],
                                [MST_KOMPONEN_GAJI].[KOMPONEN_GAJI] AS NAMA_KOMPONEN_GAJI,
                                [JUMLAH],
                                [MST_TARIF_PAYROLL].[NOMINAL] AS TARIF
                                FROM [PAYROLL].[payroll].[TBL_VAKASI]
                                JOIN [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] ON [TBL_VAKASI].[ID_KOMPONEN_GAJI] = [MST_KOMPONEN_GAJI].[ID_KOMPONEN_GAJI]
                                JOIN [PAYROLL].[simka].[MST_TARIF_PAYROLL] ON [MST_KOMPONEN_GAJI].[ID_KOMPONEN_GAJI] = [MST_TARIF_PAYROLL].[ID_KOMPONEN_GAJI]
                                WHERE [TBL_VAKASI].[ID_BULAN_GAJI] = @IdBulanGaji 
                                AND [TBL_VAKASI].[NPP] = @NPP
                                AND [TBL_VAKASI].[ID_KOMPONEN_GAJI] BETWEEN 78 AND 201";

                    var result = await conn.QueryAsync<KomponenGajiPelatihModel>(query, new { IdBulanGaji = idBulanGaji, NPP = NPP });

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                return null;
            }
        }

        public async Task<bool> InsertOrUpdateToDtlPenggajianPelatih(DetailPenggajianPelatihModel insertData)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    // Cek apakah data sudah ada
                    string checkQuery = @"SELECT COUNT(*) FROM [PAYROLL].[payroll].[DTL_PENGGAJIAN] WHERE ID_PENGGAJIAN = @ID_PENGGAJIAN AND ID_KOMPONEN_GAJI = @ID_KOMPONEN_GAJI";
                    int count = await conn.ExecuteScalarAsync<int>(checkQuery, new { insertData.ID_PENGGAJIAN, insertData.ID_KOMPONEN_GAJI });

                    if (count > 0)
                    {
                        // Update data yang sudah ada
                        string updateQuery = @"UPDATE [PAYROLL].[payroll].[DTL_PENGGAJIAN]
                                        SET [JUMLAH_SATUAN] = @JUMLAH_SATUAN, [NOMINAL] = @NOMINAL
                                        WHERE [ID_PENGGAJIAN] = @ID_PENGGAJIAN AND [ID_KOMPONEN_GAJI] = @ID_KOMPONEN_GAJI";
                        await conn.ExecuteAsync(updateQuery, insertData);
                    }
                    else
                    {
                        // Insert data baru
                        string insertQuery = @"INSERT INTO [PAYROLL].[payroll].[DTL_PENGGAJIAN]
                                        ([ID_PENGGAJIAN],[ID_KOMPONEN_GAJI],[JUMLAH_SATUAN],[NOMINAL])
                                        VALUES
                                        (@ID_PENGGAJIAN,@ID_KOMPONEN_GAJI,@JUMLAH_SATUAN,@NOMINAL)";
                        await conn.ExecuteAsync(insertQuery, insertData);
                    }

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }


        public async Task<bool> AutoHitungGajiPelatih(int idBulanGaji, string tahun, string unit)
        {
            try
            {
                var penggajianData = await GetPenggajianDataPelatih(idBulanGaji, unit); // 1. Get Data Penggajian sesuai Bulan, Tahun, Unit, dan Jenis Asisten

                if (penggajianData == null) // Jika data kosong, kembalikan false
                {
                    return false;
                }

                foreach (var penggajian in penggajianData) // 2. Iterasi untuk setiap NPP
                {
                    var komponenGajiDataList = await GetKomponenGajiMhs(idBulanGaji, penggajian.NPP); // 3. Ambil Data ID_KOMPONEN_GAJI , JUMLAH , dan TARIF

                    foreach (var komponenGajiData in komponenGajiDataList) // 4. Iterasi untuk setiap ID_KOMPONEN_GAJI
                    {
                        
                        var tarif = komponenGajiData.TARIF;
                        var nominal = tarif * komponenGajiData.JUMLAH;

                        var insertData = new DetailPenggajianPelatihModel
                        {
                            ID_PENGGAJIAN = penggajian.ID_PENGGAJIAN,
                            ID_KOMPONEN_GAJI = komponenGajiData.ID_KOMPONEN_GAJI,
                            JUMLAH_SATUAN = komponenGajiData.JUMLAH,
                            NOMINAL = nominal
                        };

                        await InsertOrUpdateToDtlPenggajianPelatih(insertData); // 5. Insert atau Update ke DTL_PENGGAJIAN
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception here
                return false;
            }
        }


        // Cetak Slip Gaji

        public async Task<bool> CheckDataGajiPelatih(int idBulanGaji)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    string query = "SELECT COUNT(*) FROM [PAYROLL].[payroll].[TBL_PENGGAJIAN] WHERE ID_BULAN_GAJI = @idBulanGaji";
                    int count = await conn.ExecuteScalarAsync<int>(query, new { idBulanGaji });
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        public async Task<IEnumerable<HeaderPenggajianPelatih>> GetHeaderPenggajianPelatih(int idBulanGaji, string unit = null)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                string query = @"SELECT
                                [TBL_PENGGAJIAN].ID_PENGGAJIAN,
                                [TBL_BULAN_GAJI].BULAN,
                                [TBL_BULAN_GAJI].ID_TAHUN,
                                [TBL_PENGGAJIAN].NPP, 
                                [TBL_PENGGAJIAN].NAMA, 
                                [TBL_PENGGAJIAN].GOLONGAN, 
                                [TBL_PENGGAJIAN].JENJANG, 
                                [TBL_PENGGAJIAN].NPWP,
                                [TBL_PENGGAJIAN].NO_TABUNGAN,
                                [TBL_PELATIH].NAMA_BANK,
                                [TBL_PELATIH].NAMA_REKENING,
                                [siatmax].[MST_UNIT].NAMA_UNIT
                                FROM [payroll].[TBL_PENGGAJIAN]
                                JOIN [payroll].[TBL_PELATIH] ON [payroll].[TBL_PENGGAJIAN].NPP = [payroll].[TBL_PELATIH].NPP
                                JOIN [siatmax].[MST_UNIT] ON [payroll].[TBL_PELATIH].ID_UNIT = [siatmax].[MST_UNIT].ID_UNIT
                                JOIN [payroll].[TBL_BULAN_GAJI] ON [payroll].[TBL_PENGGAJIAN].ID_BULAN_GAJI = [payroll].[TBL_BULAN_GAJI].ID_BULAN_GAJI 
                                WHERE [TBL_PENGGAJIAN].[ID_BULAN_GAJI] = @IdBulanGaji";

                if (!string.IsNullOrEmpty(unit))
                {
                    query += " AND [MST_UNIT].NAMA_UNIT = @Unit";
                }

                var headers = await conn.QueryAsync<HeaderPenggajianPelatih>(query, new { IdBulanGaji = idBulanGaji, Unit = unit });
                return headers;
            }
        }

        public async Task<bool> CheckDetailGajiPelatih(int idPenggajian)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                string query = "SELECT COUNT(*) FROM [PAYROLL].[payroll].[DTL_PENGGAJIAN] WHERE ID_PENGGAJIAN = @idPenggajian";
                int count = await conn.ExecuteScalarAsync<int>(query, new { idPenggajian });
                return count > 0;
            }
        }

        public async Task<IEnumerable<DetailPenggajianPelatihModel>> GetBodyPenggajianPelatih(int idPenggajian)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                string query = @"SELECT 
                                [PAYROLL].[payroll].[DTL_PENGGAJIAN].ID_KOMPONEN_GAJI, 
                                [PAYROLL].[payroll].[MST_KOMPONEN_GAJI].KOMPONEN_GAJI AS NAMA_KOMPONEN_GAJI, 
                                [PAYROLL].[payroll].[DTL_PENGGAJIAN].JUMLAH_SATUAN, 
                                [PAYROLL].[payroll].[DTL_PENGGAJIAN].NOMINAL 
                                FROM 
                                [PAYROLL].[payroll].[DTL_PENGGAJIAN]
                                JOIN 
                                [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] ON [PAYROLL].[payroll].[DTL_PENGGAJIAN].ID_KOMPONEN_GAJI = [PAYROLL].[payroll].[MST_KOMPONEN_GAJI].ID_KOMPONEN_GAJI
                                WHERE 
                                [PAYROLL].[payroll].[DTL_PENGGAJIAN].ID_PENGGAJIAN = @idPenggajian";

                var result = await conn.QueryAsync<DetailPenggajianPelatihModel>(query, new { idPenggajian });
                return result;
            }
        }





    }
}
