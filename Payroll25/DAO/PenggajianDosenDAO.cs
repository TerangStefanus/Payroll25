using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class PenggajianDosenDAO
    {
        public async Task<List<GajiKaryawanModel>> GetKaryawanData()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    string query = @"SELECT 
                                MST_KARYAWAN.NPP,
                                MST_KARYAWAN.NAMA_LENGKAP_GELAR,
                                MST_KARYAWAN.TGL_MASUK,
                                MST_KARYAWAN.ID_UNIT,
                                MST_KARYAWAN.MST_ID_UNIT,
                                MST_KARYAWAN.ID_REF_GOLONGAN,
                                MST_KARYAWAN.ID_REF_GOLONGAN_LOKAL,
                                MST_KARYAWAN.NPWP,
                                MST_KARYAWAN.STATUS_KEPEGAWAIAN,
                                MST_KARYAWAN.MASA_KERJA_GOLONGAN,
                                MST_REKENING.NO_REKENING,
                                MST_REKENING.NAMA_BANK,
                                MST_REKENING.NAMA_REKENING
                                FROM [PAYROLL].[simka].[MST_KARYAWAN]
                                INNER JOIN [PAYROLL].[simka].[MST_REKENING] ON MST_KARYAWAN.NPP = MST_REKENING.NPP
                                WHERE MST_KARYAWAN.STATUS_KEPEGAWAIAN = 'Kontrak';";

                    var result = await conn.QueryAsync<GajiKaryawanModel>(query);

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

        public async Task<bool> InsertToTblPenggajian(PenggajianDosenModel insertData)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    string query = @"INSERT INTO  [PAYROLL].[payroll].[TBL_PENGGAJIAN]
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

        public async Task<string> CalculateMasaKerjaRiil(DateTime tglMasuk)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    string query = @"DECLARE @TGL_MASUK DATETIME = @TanggalMasuk
                             DECLARE @MASA_KERJA_RIIL VARCHAR(50)

                             SELECT @MASA_KERJA_RIIL 
                            = CONVERT(VARCHAR(11), DATEDIFF(YEAR, @TGL_MASUK, GETDATE())) + '.' + CONVERT(VARCHAR(11), ABS((DATEDIFF(MONTH, @TGL_MASUK, GETDATE()) - (DATEDIFF(YEAR, @TGL_MASUK, GETDATE()) * 12))))

                             SELECT @MASA_KERJA_RIIL AS MASA_KERJA_RIIL";

                    var result = await conn.QuerySingleOrDefaultAsync<string>(query, new { TanggalMasuk = tglMasuk });
                    return result;
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

        public async Task<string> GetPangkat(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    string query = @"SELECT DESKRIPSI 
                                     FROM [simka].[REF_GOLONGAN] a 
                                     JOIN [simka].[MST_KARYAWAN] b ON a.ID_REF_GOLONGAN = b.ID_REF_GOLONGAN_LOKAL
                                     WHERE NPP = @NPP";

                    var result = await conn.QuerySingleOrDefaultAsync<string>(query, new { NPP = npp });
                    return result;
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

        public async Task<string> GetNoTabungan(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    string query = @"SELECT TOP 1 NO_REKENING
                             FROM [simka].[MST_REKENING] r
                             WHERE r.NPP = @NPP";

                    var result = await conn.QuerySingleOrDefaultAsync<string>(query, new { NPP = npp });
                    return result;
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

        // Support untuk Dropdown Insert ID_BULAN_GAJI
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


        public async Task<bool> AutoInsertPenggajian(int idBulanGaji, string tahun)
        {
            try
            {
                var karyawanData = await GetKaryawanData(); // 1. Get Data Karyawan

                foreach (var karyawan in karyawanData) // 2. Lakukan secara iterasi berdasarkan NPP karyawan
                {
                    if (!await IsDataExist(karyawan.NPP, idBulanGaji)) // 3. Cek Apakah data Penggajian dengan ID_BULAN_GAJI dan NPP sudah ada 
                    {
                        var masaKerjaRiil = await CalculateMasaKerjaRiil(karyawan.TGL_MASUK); // Hitung Masa Kerja Riil 
                        var pangkat = await GetPangkat(karyawan.NPP); // Masukkan Pangkat berdasarkan Query
                        var noTabungan = await GetNoTabungan(karyawan.NPP); // Masukan No Tabungan 

                        var insertData = new PenggajianDosenModel 
                        {
                            NPP = karyawan.NPP,
                            ID_BULAN_GAJI = idBulanGaji,
                            NAMA = karyawan.NAMA_LENGKAP_GELAR,
                            STATUS_KEPEGAWAIAN = "Kontrak",
                            MASA_KERJA_RIIL = masaKerjaRiil,
                            MASA_KERJA_GOL = karyawan.MASA_KERJA_GOLONGAN?.ToString(),  // Konversi ke string
                            JBT_STRUKTURAL = "-",
                            JBT_AKADEMIK = "-",
                            JBT_FUNGSIONAL = "-",
                            PANGKAT = pangkat,
                            GOLONGAN = karyawan.ID_REF_GOLONGAN_LOKAL,  
                            JENJANG = null,
                            NO_TABUNGAN = noTabungan,
                            NPWP = karyawan.NPWP
                        };

                        await InsertToTblPenggajian(insertData); // 4. Insert Data ke TBL_PENGGAJIAN 
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        // Auto Hitung Gaji \\

        public async Task<List<PenggajianDosenModel>> GetPenggajianData(int idBulanGaji)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    // SQL query untuk mengambil data dari TBL_PENGGAJIAN
                    string query = "SELECT [ID_PENGGAJIAN], [NPP], [ID_BULAN_GAJI] FROM [PAYROLL].[payroll].[TBL_PENGGAJIAN] WHERE STATUS_KEPEGAWAIAN = 'Kontrak' AND ID_BULAN_GAJI = @idBulanGaji";

                    var penggajianData = await conn.QueryAsync<PenggajianDosenModel>(query, new { idBulanGaji });

                    return penggajianData.ToList();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public async Task<List<KomponenGajiAndJumlahSatuanModel>> GetKomponenGajiAndJumlahSatuan(int idBulanGaji, string tahun, string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    // SQL query untuk mengambil data ID komponen gaji , jumlah satuan dan tarif
                    string query = @"
                    SELECT 
                    TBL_BEBAN_MENGAJAR.NPP, 
                    MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI,
                    MST_KOMPONEN_GAJI.KOMPONEN_GAJI AS NAMA_KOMPONEN_GAJI, 
                    TBL_BEBAN_MENGAJAR.TOTAL_SKS AS JUMLAH, 
                    MST_TARIF_PAYROLL.NOMINAL AS TARIF
                    FROM [PAYROLL].[payroll].[TBL_BEBAN_MENGAJAR]
                    INNER JOIN [PAYROLL].[simka].[MST_KARYAWAN] ON TBL_BEBAN_MENGAJAR.NPP = MST_KARYAWAN.NPP
                    JOIN [PAYROLL].[simka].[MST_TARIF_PAYROLL] ON MST_KARYAWAN.ID_REF_JBTN_AKADEMIK = MST_TARIF_PAYROLL.ID_REF_JBTN_AKADEMIK 
                    JOIN [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] ON MST_TARIF_PAYROLL.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI 
                    JOIN [PAYROLL].[simka].[REF_JENJANG] ON MST_KARYAWAN.PENDIDIKAN_TERAKHIR = REF_JENJANG.DESKRIPSI AND REF_JENJANG.ID_REF_JENJANG = MST_TARIF_PAYROLL.ID_REF_JENJANG
                    WHERE 
                        MONTH(TGL_AWAL_SK) = (SELECT ID_BULAN FROM [PAYROLL].[payroll].[TBL_BULAN_GAJI] WHERE ID_BULAN_GAJI = @ID_BULAN_GAJI) 
                        AND YEAR(TGL_AWAL_SK) = @TAHUN
	                    AND MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI = 77 
	                    AND TBL_BEBAN_MENGAJAR.NPP = @NPP

                    UNION ALL 

                    SELECT DISTINCT
                    MST_KARYAWAN.NPP,
                    MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI,
                    MST_KOMPONEN_GAJI.KOMPONEN_GAJI AS NAMA_KOMPONEN_GAJI, 
                    TBL_VAKASI.JUMLAH, 
                    MST_TARIF_PAYROLL.NOMINAL AS TARIF
                    FROM PAYROLL.simka.MST_KARYAWAN
                    JOIN PAYROLL.payroll.TBL_VAKASI ON MST_KARYAWAN.NPP = TBL_VAKASI.NPP
                    JOIN PAYROLL.payroll.MST_KOMPONEN_GAJI ON TBL_VAKASI.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI
                    JOIN PAYROLL.simka.MST_TARIF_PAYROLL ON MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI = MST_TARIF_PAYROLL.ID_KOMPONEN_GAJI
                    WHERE 
	                    TBL_VAKASI.ID_BULAN_GAJI = @ID_BULAN_GAJI
                        AND MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI BETWEEN 78 AND 201 
	                    AND MST_KARYAWAN.NPP = @NPP";

                    // Inisialisasi parameter SQL
                    var parameters = new
                    {
                        ID_BULAN_GAJI = idBulanGaji,
                        TAHUN = tahun,
                        NPP = npp
                    };

                    // Eksekusi query dan simpan hasilnya dalam sebuah list
                    var komponenGajiAndJumlahSatuanData = await conn.QueryAsync<KomponenGajiAndJumlahSatuanModel>(query, parameters);

                    return komponenGajiAndJumlahSatuanData.ToList();
                }
                catch (Exception ex)
                {
                    // Tangani pengecualian di sini
                    return null;
                }
            }
        }


        public async Task<bool> InsertOrUpdateToDtlPenggajian(DetailPenggajianModel insertData)
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


        public async Task<bool> AutoHitungGaji(int idBulanGaji, string tahun)
        {
            try
            {
                var penggajianData = await GetPenggajianData(idBulanGaji); // 1. Get Data Penggajian sesuai Bulan

                if (penggajianData == null) // 1.1 Kembalikan False ketika data kosong
                {
                    return false; 
                }

                foreach (var penggajian in penggajianData) // 2. Lakukan Iterasi untuk per npp
                {
                    var komponenGajiDataList = await GetKomponenGajiAndJumlahSatuan(idBulanGaji, tahun, penggajian.NPP); // 3. Ambil Data ID_KOMPONEN_GAJI , JUMLAH , dan TARIF

                    foreach (var komponenGajiData in komponenGajiDataList) // 4. Lakukan iterasi per ID_KOMPONEN_GAJI
                    {
                        var tarif = komponenGajiData.TARIF;  
                        var nominal = tarif * komponenGajiData.JUMLAH;

                        var insertData = new DetailPenggajianModel
                        {
                            ID_PENGGAJIAN = penggajian.ID_PENGGAJIAN,
                            ID_KOMPONEN_GAJI = komponenGajiData.ID_KOMPONEN_GAJI,
                            JUMLAH_SATUAN = komponenGajiData.JUMLAH,
                            NOMINAL = nominal
                        };

                        await InsertOrUpdateToDtlPenggajian(insertData);// 5. Insert Data ke DTL_PENGGAJIAN jika belum ada jika sudah Update ada
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

        public async Task<bool> CheckDataGaji(int idBulanGaji)
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


        public async Task<IEnumerable<HeaderPenggajian>> GetHeaderPenggajian(int idBulanGaji)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                string query = @"SELECT
                                [payroll].[TBL_PENGGAJIAN].ID_PENGGAJIAN,
                                [payroll].[TBL_PENGGAJIAN].NPP, 
                                [payroll].[TBL_PENGGAJIAN].NAMA, 
                                [payroll].[TBL_PENGGAJIAN].GOLONGAN, 
                                [payroll].[TBL_PENGGAJIAN].JENJANG, 
                                [payroll].[TBL_PENGGAJIAN].NPWP,
                                [payroll].[TBL_PENGGAJIAN].NO_TABUNGAN,
                                [simka].[MST_REKENING].NAMA_BANK,
                                [simka].[MST_REKENING].NAMA_REKENING,
                                [siatmax].[MST_UNIT].NAMA_UNIT
                                FROM 
                                    [payroll].[TBL_PENGGAJIAN]
                                JOIN 
                                    [simka].[MST_REKENING] ON [payroll].[TBL_PENGGAJIAN].NPP = [simka].[MST_REKENING].NPP
                                JOIN 
                                    [simka].[MST_KARYAWAN] ON [payroll].[TBL_PENGGAJIAN].NPP = [simka].[MST_KARYAWAN].NPP
                                JOIN 
                                    [siatmax].[MST_UNIT] ON [simka].[MST_KARYAWAN].ID_UNIT = [siatmax].[MST_UNIT].ID_UNIT
                                WHERE 
                                    [payroll].[TBL_PENGGAJIAN].ID_BULAN_GAJI = @idBulanGaji ";

                var headers = await conn.QueryAsync<HeaderPenggajian>(query, new { idBulanGaji });
                return headers;
            }
        }

        public async Task<bool> CheckDetailGaji(int idPenggajian)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                string query = "SELECT COUNT(*) FROM [PAYROLL].[payroll].[DTL_PENGGAJIAN] WHERE ID_PENGGAJIAN = @idPenggajian";
                int count = await conn.ExecuteScalarAsync<int>(query, new { idPenggajian });
                return count > 0;
            }
        }

        public async Task<IEnumerable<DetailPenggajianModel>> GetBodyPenggajian(int idPenggajian)
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

                var result = await conn.QueryAsync<DetailPenggajianModel>(query, new { idPenggajian });
                return result;
            }
        }

    }
}
