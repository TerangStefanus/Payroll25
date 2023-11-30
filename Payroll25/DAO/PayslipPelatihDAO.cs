using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Reflection.PortableExecutable;
using Dapper;
using Payroll25.Models;

namespace Payroll25.DAO
{
    public class PayslipPelatihDAO
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

        public async Task<bool> CheckDataGajiUserPelatih(int idBulanGaji, string npp)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    string query = "SELECT COUNT(*) FROM [PAYROLL].[payroll].[TBL_PENGGAJIAN] JOIN [PAYROLL].[payroll].[TBL_PELATIH] ON TBL_PENGGAJIAN.NPP = TBL_PELATIH.NPP WHERE ID_BULAN_GAJI = @idBulanGaji AND TBL_PENGGAJIAN.NPP = @npp";
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

        public async Task<IEnumerable<HeaderPenggajianUserPelatih>> GetHeaderPenggajianUserPelatih(int idBulanGaji, string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                string query = @"SELECT
                                [payroll].[TBL_PENGGAJIAN].ID_PENGGAJIAN,
                                [TBL_BULAN_GAJI].BULAN,
                                [TBL_BULAN_GAJI].ID_TAHUN,
                                [payroll].[TBL_PENGGAJIAN].NPP, 
                                [payroll].[TBL_PENGGAJIAN].NAMA, 
                                [payroll].[TBL_PENGGAJIAN].GOLONGAN, 
                                [payroll].[TBL_PENGGAJIAN].JENJANG, 
                                [payroll].[TBL_PENGGAJIAN].NPWP,
                                [payroll].[TBL_PENGGAJIAN].NO_TABUNGAN,
                                [payroll].[TBL_PELATIH].NAMA_BANK,
                                [payroll].[TBL_PELATIH].NAMA_REKENING,
                                [siatmax].[MST_UNIT].NAMA_UNIT
                                FROM 
                                    [payroll].[TBL_PENGGAJIAN]
                                JOIN 
                                    [payroll].[TBL_PELATIH] ON [payroll].[TBL_PENGGAJIAN].NPP = [payroll].[TBL_PELATIH].NPP
                                JOIN 
                                    [siatmax].[MST_UNIT] ON [payroll].[TBL_PELATIH].ID_UNIT = [siatmax].[MST_UNIT].ID_UNIT
                                JOIN
	                                [payroll].[TBL_BULAN_GAJI] ON [payroll].[TBL_PENGGAJIAN].ID_BULAN_GAJI = [payroll].[TBL_BULAN_GAJI].ID_BULAN_GAJI 
                                WHERE 
                                    [payroll].[TBL_PENGGAJIAN].ID_BULAN_GAJI = @idBulanGaji AND TBL_PENGGAJIAN.NPP = @npp";

                var headers = await conn.QueryAsync<HeaderPenggajianUserPelatih>(query, new { idBulanGaji, npp });
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

        public async Task<IEnumerable<DetailPenggajianUserPelatihModel>> GetBodyPenggajianUserPelatih(int idPenggajian)
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

                var result = await conn.QueryAsync<DetailPenggajianUserPelatihModel>(query, new { idPenggajian });
                return result;
            }
        }



    }
}
