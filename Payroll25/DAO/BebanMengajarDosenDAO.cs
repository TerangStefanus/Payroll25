﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Payroll25.Models;
using static Payroll25.Models.BebanMengajarDosenModel;

namespace Payroll25.DAO
{
    public class BebanMengajarDosenDAO
    {
        public async Task<BebanMengajarDosenModel> ShowBebanMengajarAsync()
        {
            var connectionString = DBkoneksi.payrollkoneksi;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    var query = @"SELECT TOP 1 
                                BM.ID_BEBAN_MENGAJAR, 
                                K.NAMA, 
                                BM.NPP, 
                                BM.ID_TAHUN_AKADEMIK, 
                                BM.NO_SEMESTER, 
                                BM.TOTAL_SKS,
                                TP.NAMA_TARIF_PAYROLL AS BEBAN_GELAR, 
                                TP.NOMINAL AS TARIF,
                                CONVERT(varchar, BM.TGL_AWAL_SK, 23) AS TGL_AWAL_SK,
                                CONVERT(varchar, BM.TGL_AKHIR_SK, 23) AS TGL_AKHIR_SK
                                FROM 
                                    [PAYROLL].[payroll].[TBL_BEBAN_MENGAJAR] AS BM
                                INNER JOIN 
                                    [PAYROLL].[simka].[MST_KARYAWAN] AS K ON BM.NPP = K.NPP
                                LEFT JOIN 
                                    [PAYROLL].[simka].[MST_TARIF_PAYROLL] AS TP ON K.ID_REF_JBTN_AKADEMIK = TP.ID_REF_JBTN_AKADEMIK
                               ORDER BY BM.TGL_AWAL_SK DESC";

                    return (await conn.QueryAsync<BebanMengajarDosenModel>(query)).FirstOrDefault();
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

        public int UpdateBebanMengajar(List<BebanMengajarDosenModel> model)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"UPDATE [PAYROLL].[payroll].[TBL_BEBAN_MENGAJAR]
                                SET [TOTAL_SKS] = @TOTAL_SKS
                                WHERE ID_BEBAN_MENGAJAR = @ID_BEBAN_MENGAJAR";

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







    }

}
