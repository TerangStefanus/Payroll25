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
                var karyawanData = await GetKaryawanData();

                foreach (var karyawan in karyawanData)
                {
                    if (!await IsDataExist(karyawan.NPP, idBulanGaji))
                    {
                        var masaKerjaRiil = await CalculateMasaKerjaRiil(karyawan.TGL_MASUK);
                        var pangkat = await GetPangkat(karyawan.NPP);
                        var noTabungan = await GetNoTabungan(karyawan.NPP);

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
                            GOLONGAN = karyawan.ID_REF_GOLONGAN_LOKAL,  // Konversi ke string
                            JENJANG = null,
                            NO_TABUNGAN = noTabungan,
                            NPWP = karyawan.NPWP
                        };

                        await InsertToTblPenggajian(insertData);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
