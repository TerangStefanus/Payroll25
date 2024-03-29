﻿using Dapper;
using Payroll25.DAO;
using Payroll25.Models;
using System.Data.SqlClient;
using System.Drawing;

namespace Payroll25.DAO
{
    public class PenggajianAsistenDAO
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

        public async Task<List<string>> GetJenis()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    await conn.OpenAsync();
                    const string query = @"SELECT [JENIS] FROM [PAYROLL].[payroll].[REF_JENIS_ASISTEN]";
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


        public async Task<List<AsistenDataModel>> GetAsistenData(string unit, string jenis)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    await conn.OpenAsync();

                    const string query = @"SELECT DISTINCT
                                    [PAYROLL].[payroll].[TBL_ASISTEN].ID_ASISTEN, 
                                    [PAYROLL].[payroll].[TBL_ASISTEN].ID_TAHUN_AKADEMIK, 
                                    [PAYROLL].[payroll].[TBL_ASISTEN].NO_SEMESTER, 
                                    [PAYROLL].[payroll].[TBL_ASISTEN].NPM,
                                    MHS.nama_mhs AS NAMA_MHS,
                                    [PAYROLL].[payroll].[TBL_ASISTEN].ID_UNIT,
                                    [PAYROLL].[siatmax].[MST_UNIT].NAMA_UNIT,
                                    [PAYROLL].[payroll].[TBL_ASISTEN].NO_REKENING, 
                                    [PAYROLL].[payroll].[TBL_ASISTEN].NAMA_REKENING, 
                                    [PAYROLL].[payroll].[TBL_ASISTEN].NAMA_BANK,
                                    REF.JENIS AS JENIS
                                    FROM 
                                    [PAYROLL].[siatmax].[MST_UNIT]
                                    INNER JOIN [PAYROLL].[payroll].[TBL_ASISTEN] ON [PAYROLL].[siatmax].[MST_UNIT].ID_UNIT = [PAYROLL].[payroll].[TBL_ASISTEN].ID_UNIT
                                    INNER JOIN [PAYROLL].[dbo].[mst_mhs_aktif] AS MHS ON [PAYROLL].[payroll].[TBL_ASISTEN].NPM = MHS.npm
                                    INNER JOIN [PAYROLL].[payroll].[REF_JENIS_ASISTEN] AS REF ON [PAYROLL].[payroll].[TBL_ASISTEN].ID_JENIS_ASISTEN = REF.ID_JENIS_ASISTEN
                                    WHERE MST_UNIT.NAMA_UNIT = @Unit AND REF.JENIS = @Jenis
                                    ORDER BY [PAYROLL].[payroll].[TBL_ASISTEN].ID_ASISTEN DESC;";

                    var parameters = new { Unit = unit, Jenis = jenis };

                    var result = await conn.QueryAsync<AsistenDataModel>(query, parameters);

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

        public async Task<bool> InsertToTblPenggajianMhs(PenggajianAsistenModel insertData)
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

        public async Task<bool> AutoInsertPenggajianAsisten(int idBulanGaji, string tahun, string unit, string jenis)
        {
            try
            {
                var asistenData = await GetAsistenData(unit, jenis); // 1. Get Data Asisten

                foreach (var asisten in asistenData) // 2. Lakukan secara iterasi berdasarkan NPM asisten
                {
                    var insertData = new PenggajianAsistenModel
                    {
                        NPP = asisten.NPM,
                        ID_BULAN_GAJI = idBulanGaji,
                        NAMA = asisten.NAMA_MHS,
                        STATUS_KEPEGAWAIAN = "Kontrak",
                        MASA_KERJA_RIIL = "0",
                        MASA_KERJA_GOL = "0",
                        JBT_STRUKTURAL = "-",
                        JBT_AKADEMIK = "-",
                        JBT_FUNGSIONAL = "-",
                        PANGKAT = "S0",
                        GOLONGAN = "II/A",
                        JENJANG = "-",
                        NO_TABUNGAN = asisten.NO_REKENING,
                        NPWP = null
                    };

                    await InsertToTblPenggajianMhs(insertData); // 4. Insert Data ke TBL_PENGGAJIAN 
                }

                return true;
            }
            catch (Exception ex)
            {
                // Consider logging the exception here
                return false;
            }
        }


        // Auto Hitung Gaji Asisten / Mahasiswa

        public async Task<IEnumerable<PenggajianAsistenModel>> GetPenggajianDataMhs(int idBulanGaji, string unit)
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
                                [NAMA], 
                                [STATUS_KEPEGAWAIAN], 
                                [PANGKAT], 
                                [GOLONGAN], 
                                [JENJANG], 
                                [NO_TABUNGAN], 
                                TBL_PENGGAJIAN.[NPWP]
                                FROM [PAYROLL].[payroll].[TBL_PENGGAJIAN]
                                JOIN [PAYROLL].[payroll].[TBL_ASISTEN] ON [TBL_PENGGAJIAN].NPP = [TBL_ASISTEN].NPM
                                JOIN [PAYROLL].[siatmax].[MST_UNIT] ON [TBL_ASISTEN].ID_UNIT = [MST_UNIT].ID_UNIT
                                JOIN [PAYROLL].[payroll].[REF_JENIS_ASISTEN] ON [TBL_ASISTEN].ID_JENIS_ASISTEN = [REF_JENIS_ASISTEN].ID_JENIS_ASISTEN
                                WHERE [TBL_PENGGAJIAN].[ID_BULAN_GAJI] = @IdBulanGaji AND [MST_UNIT].NAMA_UNIT = @Unit
                                ORDER BY [TBL_PENGGAJIAN].ID_PENGGAJIAN DESC;";

                    var result = await conn.QueryAsync<PenggajianAsistenModel>(query, new { IdBulanGaji = idBulanGaji, Unit = unit });

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                return null;
            }
        }


        public async Task<IEnumerable<KomponenGajiMhsModel>> GetKomponenGajiMhs(int idBulanGaji, string NPP)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    await conn.OpenAsync();

                    var query = @"SELECT * FROM (
                                  SELECT TOP 1
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
                                  AND [TBL_VAKASI].[ID_KOMPONEN_GAJI] = 77
                                  AND MST_TARIF_PAYROLL.NOMINAL = 78000
                                  ORDER BY [MST_TARIF_PAYROLL].[NOMINAL] DESC ) AS A

                                  UNION ALL

                                  SELECT * FROM (
                                  SELECT
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
                                  AND [TBL_VAKASI].[ID_KOMPONEN_GAJI] BETWEEN 78 AND 201) AS B";

                    var result = await conn.QueryAsync<KomponenGajiMhsModel>(query, new { IdBulanGaji = idBulanGaji, NPP = NPP });

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                return null;
            }
        }

        // Logika masih kurang tepat bisa ditinjau ulang codenya 
        // tidak ada logika menghapus data komponen gaji yang sudah terlanjur dimasukkan ( pastikan logika bisnisnya )
        public async Task<bool> InsertOrUpdateToDtlPenggajianMhs(DetailPenggajianMhsModel insertData)
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

        public async Task<bool> AutoHitungGajiAsisten(int idBulanGaji, string tahun, string unit, string jenis)
        {
            try
            {

                var penggajianData = await GetPenggajianDataMhs(idBulanGaji, unit);
             
                foreach (var penggajian in penggajianData) // Iterasi untuk setiap NPP
                {
                    if (penggajianData == null || !penggajianData.Any()) continue; // jika tidak data penggajian , keluar dari foreach 

                    var komponenGajiDataList = await GetKomponenGajiMhs(idBulanGaji, penggajian.NPP); // Ambil Data ID_KOMPONEN_GAJI , JUMLAH , dan TARIF

                    foreach (var komponenGajiData in komponenGajiDataList) // Iterasi untuk setiap ID_KOMPONEN_GAJI
                    {

                        var tarif = komponenGajiData.TARIF;
                        var nominal = tarif * komponenGajiData.JUMLAH;

                        var insertData = new DetailPenggajianMhsModel
                        {
                            ID_PENGGAJIAN = penggajian.ID_PENGGAJIAN,
                            ID_KOMPONEN_GAJI = komponenGajiData.ID_KOMPONEN_GAJI,
                            JUMLAH_SATUAN = komponenGajiData.JUMLAH,
                            NOMINAL = nominal
                        };

                        await InsertOrUpdateToDtlPenggajianMhs(insertData); // Insert atau Update ke DTL_PENGGAJIAN
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

        public async Task<bool> CheckDataGajiMhs(int idBulanGaji)
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

        public async Task<IEnumerable<HeaderPenggajianMhs>> GetHeaderPenggajianAsisten(int idBulanGaji, string unit, string jenis)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                string query = @"SELECT
                                [TBL_PENGGAJIAN].ID_PENGGAJIAN,
                                [TBL_BULAN_GAJI].BULAN,
                                [TBL_BULAN_GAJI].ID_TAHUN,
                                [TBL_PENGGAJIAN].NPP, 
                                [TBL_PENGGAJIAN].NAMA,
                                [TBL_PENGGAJIAN].PANGKAT,
                                [TBL_PENGGAJIAN].GOLONGAN, 
                                [TBL_PENGGAJIAN].JENJANG, 
                                [TBL_PENGGAJIAN].NPWP,
                                [TBL_PENGGAJIAN].NO_TABUNGAN,
                                [TBL_ASISTEN].NAMA_BANK,
                                [TBL_ASISTEN].NAMA_REKENING,
                                [siatmax].[MST_UNIT].NAMA_UNIT,
                                [REF_JENIS_ASISTEN].JENIS
                                FROM [payroll].[TBL_PENGGAJIAN]
                                JOIN [payroll].[TBL_ASISTEN] ON [payroll].[TBL_PENGGAJIAN].NPP = [payroll].[TBL_ASISTEN].NPM
                                JOIN [siatmax].[MST_UNIT] ON [payroll].[TBL_ASISTEN].ID_UNIT = [siatmax].[MST_UNIT].ID_UNIT
                                JOIN [PAYROLL].[payroll].[REF_JENIS_ASISTEN] ON [TBL_ASISTEN].ID_JENIS_ASISTEN = [REF_JENIS_ASISTEN].ID_JENIS_ASISTEN
                                JOIN [payroll].[TBL_BULAN_GAJI] ON [payroll].[TBL_PENGGAJIAN].ID_BULAN_GAJI = [payroll].[TBL_BULAN_GAJI].ID_BULAN_GAJI 
                                WHERE [TBL_PENGGAJIAN].[ID_BULAN_GAJI] = @IdBulanGaji 
                                AND [MST_UNIT].NAMA_UNIT = @Unit 
                                AND [REF_JENIS_ASISTEN].JENIS = @Jenis";

                var headers = await conn.QueryAsync<HeaderPenggajianMhs>(query, new { IdBulanGaji = idBulanGaji, Unit = unit, Jenis = jenis });
                return headers;
            }
        }

        public async Task<bool> CheckDetailGajiAsisten(int idPenggajian)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                string query = "SELECT COUNT(*) FROM [PAYROLL].[payroll].[DTL_PENGGAJIAN] WHERE ID_PENGGAJIAN = @idPenggajian";
                int count = await conn.ExecuteScalarAsync<int>(query, new { idPenggajian });
                return count > 0;
            }
        }

        public async Task<IEnumerable<DetailPenggajianMhsModel>> GetBodyPenggajianAsisten(int idPenggajian)
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

                var result = await conn.QueryAsync<DetailPenggajianMhsModel>(query, new { idPenggajian });
                return result;
            }
        }

        public async Task<decimal> GetTarifPajakByNPWPStatus(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                string query = @"SELECT 
                                CASE 
                                    WHEN TBL_ASISTEN.NPWP IS NOT NULL THEN 1
                                    ELSE 0
                                END AS STATUS_NPWP,
                                ISNULL
                                (
                                    CASE 
                                        WHEN TBL_ASISTEN.NPWP IS NOT NULL THEN (SELECT [NOMINAL] FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] WHERE ID_MST_TARIF_PAYROLL = 1182)
                                        ELSE (SELECT [NOMINAL] FROM [PAYROLL].[simka].[MST_TARIF_PAYROLL] WHERE ID_MST_TARIF_PAYROLL = 1183)
                                    END, 0
                                ) 
                                AS TARIF_PAJAK
                                FROM 
                                    [PAYROLL].[payroll].[TBL_ASISTEN]
                                WHERE 
                                    TBL_ASISTEN.NPM = @npp";

                var result = await conn.QueryFirstOrDefaultAsync<(int StatusNPWP, decimal TarifPajak)>(query, new { npp });

                return result.TarifPajak;
            }
        }

        public async Task<byte[]> GetTandaTanganKSDM()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                string query = @"SELECT [IMG_TTD_PEJABAT]
                                FROM [PAYROLL].[siatmax].[MST_UNIT]
                                WHERE NAMA_UNIT LIKE '%Kepala Kantor Sumber Daya Manusia%'";

                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Pastikan untuk menyesuaikan nama kolom dan tipenya sesuai dengan database Anda
                            if (!reader.IsDBNull(0))
                            {
                                // Ambil byte array dari hasil query
                                byte[] imageBytes = (byte[])reader[0];
                                return imageBytes;
                            }
                        }
                    }
                }
            }

            return null; // Return null jika data tidak ditemukan
        }


        public async Task<string> GetNamaKepalaKSDM()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                conn.Open();

                string query = @"SELECT [PAYROLL].[simka].[MST_KARYAWAN].[NAMA_LENGKAP_GELAR]
                               FROM [PAYROLL].[siatmax].[MST_UNIT]
                               JOIN [PAYROLL].[simka].[MST_KARYAWAN] 
                               ON [PAYROLL].[siatmax].[MST_UNIT].[NPP] = [PAYROLL].[simka].[MST_KARYAWAN].[NPP]
                               WHERE [PAYROLL].[siatmax].[MST_UNIT].[NAMA_UNIT] LIKE '%Kepala Kantor Sumber Daya Manusia%';";

                var result = await conn.QueryFirstOrDefaultAsync<string>(query);

                return result;
            }
        }








    }
}
