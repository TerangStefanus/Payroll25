﻿using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Reflection.PortableExecutable;
using Dapper;
using Payroll25.Models;

namespace Payroll25.DAO
{
    public class PayslipDAO
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

        public async Task<bool> CheckDataGajiMhs(int idBulanGaji, string npp)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    string query = "SELECT COUNT(*) \r\nFROM (\r\n    SELECT DISTINCT \r\n        TBL_PENGGAJIAN.ID_PENGGAJIAN,\r\n        TBL_PENGGAJIAN.NPP,\r\n        TBL_PENGGAJIAN.ID_BULAN_GAJI\r\n    FROM [PAYROLL].[payroll].[TBL_PENGGAJIAN] \r\n    JOIN [PAYROLL].[payroll].[TBL_ASISTEN] ON TBL_PENGGAJIAN.NPP = TBL_ASISTEN.NPM \r\n    WHERE TBL_PENGGAJIAN.ID_BULAN_GAJI = @idBulanGaji AND TBL_PENGGAJIAN.NPP = @npp\r\n) AS SUBQUERY";
                    int count = await conn.ExecuteScalarAsync<int>(query, new { IdBulanGaji = idBulanGaji, npp = npp });
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        public async Task<IEnumerable<AsistenDataModel>> GetAsistenDataByNPP(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                string query = @"SELECT TOP (1) 
                                [ID_ASISTEN],
                                [ID_TAHUN_AKADEMIK],
                                [NO_SEMESTER],
                                [NPM],
                                [ID_UNIT],
                                [NO_REKENING],
                                [NAMA_REKENING],
                                [NAMA_BANK],
                                TBL_ASISTEN.[ID_JENIS_ASISTEN],
                                REF_JENIS_ASISTEN.JENIS
                                FROM [PAYROLL].[payroll].[TBL_ASISTEN]
                                JOIN [PAYROLL].[payroll].[REF_JENIS_ASISTEN] ON TBL_ASISTEN.ID_JENIS_ASISTEN = REF_JENIS_ASISTEN.ID_JENIS_ASISTEN
                                WHERE NPM =  @npp";

                var asistenData = await conn.QueryAsync<AsistenDataModel>(query, new { npp = npp });
                return asistenData;
            }
        }

        public static string ConvertJenisToPangkat(string jenis)
        {
            switch (jenis)
            {
                case "Asisten Mahasiswa":
                    return "1";
                case "Asisten Lab":
                    return "2";
                case "Student Staf":
                    return "3";
                default:
                    throw new ArgumentException("Jenis not recognized");
            }
        }


        public async Task<IEnumerable<HeaderPenggajianUserAsistenModel>> GetHeaderPenggajianUserAsisten(int idBulanGaji, string npp , string jenis)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                string pangkat = ConvertJenisToPangkat(jenis); // Konversi jenis ke pangkat

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
                                [PAYROLL].[siatmax].[MST_UNIT].NAMA_UNIT,
                                [REF_JENIS_ASISTEN].JENIS
                                FROM [PAYROLL].[payroll].[TBL_PENGGAJIAN]
                                JOIN [PAYROLL].[payroll].[TBL_ASISTEN] ON [PAYROLL].[payroll].[TBL_PENGGAJIAN].NPP = [PAYROLL].[payroll].[TBL_ASISTEN].NPM
                                JOIN [PAYROLL].[siatmax].[MST_UNIT] ON [PAYROLL].[payroll].[TBL_ASISTEN].ID_UNIT = [PAYROLL].[siatmax].[MST_UNIT].ID_UNIT
                                JOIN [PAYROLL].[payroll].[REF_JENIS_ASISTEN] ON [TBL_ASISTEN].ID_JENIS_ASISTEN = [REF_JENIS_ASISTEN].ID_JENIS_ASISTEN
                                JOIN [payroll].[TBL_BULAN_GAJI] ON [payroll].[TBL_PENGGAJIAN].ID_BULAN_GAJI = [payroll].[TBL_BULAN_GAJI].ID_BULAN_GAJI 
                                WHERE [TBL_PENGGAJIAN].[ID_BULAN_GAJI] = @IdBulanGaji 
                                AND [REF_JENIS_ASISTEN].JENIS = @Jenis
                                AND NPM = @npp";

                var headers = await conn.QueryAsync<HeaderPenggajianUserAsistenModel>(query, new { IdBulanGaji = idBulanGaji , npp = npp , Jenis = jenis });
                return headers;
                  
            }
        }


        public async Task<bool> CheckDetailGajiAsisten(int idPenggajian)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    string query = "SELECT COUNT(*) FROM [PAYROLL].[payroll].[DTL_PENGGAJIAN] WHERE ID_PENGGAJIAN = @idPenggajian";
                    int count = await conn.ExecuteScalarAsync<int>(query, new { idPenggajian });
                    return count > 0;
                }
                catch (Exception ex)
                {
                    // Handle exception, you might want to log it or handle it in a way that's appropriate for your application
                    return false;
                }
            }
        }


        public async Task<IEnumerable<BodyDetailPenggajianUserAsistenModel>> GetBodyPenggajianAsisten(int idPenggajian)
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

                var result = await conn.QueryAsync<BodyDetailPenggajianUserAsistenModel>(query, new { idPenggajian });
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
