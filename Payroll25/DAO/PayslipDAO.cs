using System;
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
                    string query = "SELECT COUNT(*) FROM [PAYROLL].[payroll].[TBL_PENGGAJIAN] JOIN [PAYROLL].[payroll].[TBL_ASISTEN] ON TBL_PENGGAJIAN.NPP = TBL_ASISTEN.NPM WHERE ID_BULAN_GAJI = @idBulanGaji AND TBL_PENGGAJIAN.NPP = @npp";
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

        public async Task<IEnumerable<HeaderPenggajianUserAsistenModel>> GetHeaderPenggajianAsisten(int idBulanGaji, string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {

                string query = @"SELECT 
                        TBL_PENGGAJIAN.ID_PENGGAJIAN,
                        TBL_PENGGAJIAN.NPP,
                        TBL_PENGGAJIAN.ID_BULAN_GAJI,
                        TBL_PENGGAJIAN.NAMA,
                        TBL_PENGGAJIAN.STATUS_KEPEGAWAIAN,
                        TBL_PENGGAJIAN.GOLONGAN,
                        TBL_PENGGAJIAN.JENJANG,
                        TBL_PENGGAJIAN.NO_TABUNGAN,
                        TBL_PENGGAJIAN.NPWP,
                        TBL_ASISTEN.ID_ASISTEN,
                        TBL_ASISTEN.ID_TAHUN_AKADEMIK,
                        TBL_ASISTEN.NO_SEMESTER,
                        TBL_ASISTEN.NPM,
                        TBL_ASISTEN.ID_UNIT,
                        TBL_ASISTEN.NO_REKENING,
                        TBL_ASISTEN.NAMA_REKENING,
                        TBL_ASISTEN.NAMA_BANK,
                        TBL_ASISTEN.ID_JENIS_ASISTEN,
                        REF_JENIS_ASISTEN.JENIS,
                        MST_UNIT.NAMA_UNIT
                        FROM [PAYROLL].[payroll].[TBL_PENGGAJIAN]
                        JOIN [PAYROLL].[payroll].[TBL_ASISTEN] ON TBL_PENGGAJIAN.NPP = TBL_ASISTEN.NPM
                        JOIN [PAYROLL].[payroll].[REF_JENIS_ASISTEN] ON TBL_ASISTEN.ID_JENIS_ASISTEN = REF_JENIS_ASISTEN.ID_JENIS_ASISTEN
                        JOIN [PAYROLL].[siatmax].[MST_UNIT] ON TBL_ASISTEN.ID_UNIT = MST_UNIT.ID_UNIT
                        WHERE TBL_PENGGAJIAN.ID_BULAN_GAJI = @idBulanGaji AND TBL_PENGGAJIAN.NPP = @npp"
                ;

                var headers = await conn.QueryAsync<HeaderPenggajianUserAsistenModel>(query, new { IdBulanGaji = idBulanGaji , npp = npp });
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




    }
}
