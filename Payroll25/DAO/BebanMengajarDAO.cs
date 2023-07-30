using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Payroll25.Models;

namespace Payroll25.DAO
{
    public class BebanMengajarDAO
    {
        public async Task<IEnumerable<BebanMengajarModel>> ShowBebanMengajarAsync(string prodi = null, string namaMK = null, string fakultas = null)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var parameters = new DynamicParameters();
                    string query = @"SELECT ";

                    if (!string.IsNullOrEmpty(prodi) || !string.IsNullOrEmpty(namaMK) || !string.IsNullOrEmpty(fakultas))
                    {
                        // Data diambil 1000 atau kurang karena menghindari penggunaan memory berlebih
                        query += "TOP 1000 ";
                    }
                    else
                    {
                        // Data diambil 0 karena menghindari out of memory
                        query += "TOP 0 ";
                    }

                    query += @"MKARY.[NPP],
                            KLS.[KODE_MK],
                            MK.[SKS],
                            MK.[NAMA_MK],
                            KLS.[KELAS],
                            CONVERT(varchar, BM.[TGL_AWAL_SK], 23) AS AWAL,
                            CONVERT(varchar, BM.[TGL_AKHIR_SK], 23) AS AKHIR
                            FROM
                            [PAYROLL].[simka].[MST_KARYAWAN] AS MKARY
                            JOIN
                            [PAYROLL].[dbo].[tbl_kelas] AS KLS ON MKARY.[NPP] = KLS.[NPP_DOSEN1]
                            JOIN
                            [PAYROLL].[dbo].[tbl_matakuliah] AS MK ON KLS.[KODE_MK] = MK.[KODE_MK]
                            JOIN
                            [PAYROLL].[payroll].[TBL_BEBAN_MENGAJAR] AS BM ON MKARY.[NPP] = BM.[NPP]
                            JOIN
                            [PAYROLL].[dbo].[tbl_semester_akademik] AS SA ON KLS.[ID_TAHUN_AKADEMIK] = SA.[ID_TAHUN_AKADEMIK]
                            JOIN
                            [PAYROLL].[dbo].[ref_prodi] AS PRD ON PRD.[ID_PRODI] = KLS.[ID_PRODI_BUAT]
                            JOIN
                            [PAYROLL].[dbo].[ref_fakultas] AS FKL ON FKL.[ID_FAKULTAS] = PRD.[ID_FAKULTAS]";

                    if (!string.IsNullOrEmpty(prodi))
                    {
                        query += " WHERE PRD.[PRODI] = @Prodi";
                        parameters.Add("@Prodi", prodi);
                    }

                    if (!string.IsNullOrEmpty(namaMK))
                    {
                        if (!string.IsNullOrEmpty(prodi))
                        {
                            query += " AND";
                        }
                        else
                        {
                            query += " WHERE";
                        }

                        query += " MK.[NAMA_MK] LIKE @NamaMK";
                        parameters.Add("@NamaMK", $"%{namaMK}%");
                    }

                    if (!string.IsNullOrEmpty(fakultas))
                    {
                        if (!string.IsNullOrEmpty(prodi) || !string.IsNullOrEmpty(namaMK))
                        {
                            query += " AND";
                        }
                        else
                        {
                            query += " WHERE";
                        }

                        query += " FKL.[FAKULTAS] LIKE @Fakultas";
                        parameters.Add("@Fakultas", $"%{fakultas}%");
                    }

                    query += " ORDER BY KLS.[KODE_MK] ASC ";

                    var data = await conn.QueryAsync<BebanMengajarModel>(query, parameters);
                    return data;
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return null;
                }
            }
        }

    }
}
