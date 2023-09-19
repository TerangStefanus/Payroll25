using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class DataGajiDAO
    {

        public async Task<IEnumerable<DataGajiModel>> ListUnit()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"SELECT 
                                  [ID_UNIT],
                                  [NAMA_UNIT]
                                  FROM [PAYROLL].[siatmax].[MST_UNIT] ";

                    var data = await conn.QueryAsync<DataGajiModel>(query);

                    return data.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<DataGajiModel>();
                }
            }
        }

        public async Task<IEnumerable<DataGajiModel>> GetUnit(int IDUnit)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"SELECT 
                                  [NAMA_UNIT]
                                  FROM [PAYROLL].[siatmax].[MST_UNIT] 
                                  WHERE ID_UNIT = @IDUnit";

                    var data = await conn.QueryAsync<DataGajiModel>(query, new { IDUnit });

                    return data.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<DataGajiModel>();
                }
            }
        }

        public async Task<IEnumerable<DataGajiModel>> GetBulan(int IDBulan)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"SELECT [BULAN]
                                FROM [PAYROLL].[payroll].[REF_BULAN]
                                WHERE ID_BULAN = @IDBulan";

                    var data = await conn.QueryAsync<DataGajiModel>(query, new { IDBulan });

                    return data.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<DataGajiModel>();
                }
            }
        }

        public async Task<IEnumerable<DataGajiModel>> GetDataKaryawan(int IDPenggajian)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"SELECT 
                                [PAYROLL].[payroll].[TBL_PENGGAJIAN].[NPP], 
                                [PAYROLL].[payroll].[TBL_PENGGAJIAN].[NAMA], 
                                [PAYROLL].[payroll].[TBL_PENGGAJIAN].[GOLONGAN], 
                                [PAYROLL].[payroll].[TBL_PENGGAJIAN].[JENJANG], 
                                [PAYROLL].[payroll].[TBL_PENGGAJIAN].[NPWP], 
                                [PAYROLL].[simka].[MST_REKENING].[NO_REKENING], 
                                [PAYROLL].[simka].[MST_REKENING].[NAMA_BANK], 
                                [PAYROLL].[simka].[MST_REKENING].[NAMA_REKENING]
                                FROM 
                                    [PAYROLL].[payroll].[TBL_PENGGAJIAN]
                                INNER JOIN 
                                    [PAYROLL].[simka].[MST_REKENING]
                                ON 
                                    [PAYROLL].[payroll].[TBL_PENGGAJIAN].[NPP] = [PAYROLL].[simka].[MST_REKENING].[NPP]
                                WHERE TBL_PENGGAJIAN.ID_PENGGAJIAN = @IDPenggajian";

                    var data = await conn.QueryAsync<DataGajiModel>(query, new { IDPenggajian });

                    return data.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<DataGajiModel>();
                }
            }
        }

        public async Task<List<DetailGaji>> GetDetailGaji(int IDPenggajian)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"SELECT 
                        MST_KOMPONEN_GAJI.KOMPONEN_GAJI,
                        DTL_PENGGAJIAN.[JUMLAH_SATUAN],
                        DTL_PENGGAJIAN.[NOMINAL]
                        FROM [PAYROLL].[payroll].[DTL_PENGGAJIAN]
                        INNER JOIN [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] ON DTL_PENGGAJIAN.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI
                        WHERE DTL_PENGGAJIAN.ID_PENGGAJIAN = @IDPenggajian";

                    var data = await conn.QueryAsync<DetailGaji>(query, new { IDPenggajian });

                    return data.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<DetailGaji>().ToList();
                }
            }
        }







    }
}
