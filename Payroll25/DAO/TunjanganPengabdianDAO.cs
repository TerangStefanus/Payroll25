﻿using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class TunjanganPengabdianDAO
    {
        public async Task<IEnumerable<TunjanganPengabdianModel>> ShowTunjanganPengabdianAsync(string prodi = null, string namaMK = null, string fakultas = null)
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
                        query += "TOP 50 ";
                    }

                    query += @"MST_KARYAWAN.NPP,
                               tbl_matakuliah.SKS,
                               tbl_matakuliah.NAMA_MK,
                               tbl_kelas.KELAS,
                               TBL_VAKASI.JUMLAH AS Jml_Hadir,
                               TBL_VAKASI.DATE_INSERTED AS Tgl_buat,
                               ref_prodi.ID_UNIT AS Kode_unit
                               FROM PAYROLL.simka.MST_KARYAWAN
                               JOIN PAYROLL.dbo.tbl_kelas ON MST_KARYAWAN.NPP = tbl_kelas.NPP_DOSEN1
                               JOIN PAYROLL.dbo.tbl_matakuliah ON tbl_kelas.ID_MK = tbl_matakuliah.ID_MK
                               JOIN PAYROLL.payroll.TBL_VAKASI ON MST_KARYAWAN.NPP = TBL_VAKASI.NPP
                               JOIN PAYROLL.dbo.ref_prodi ON tbl_matakuliah.ID_PRODI = ref_prodi.ID_PRODI";

                    if (!string.IsNullOrEmpty(prodi))
                    {
                        query += " WHERE ref_prodi.[PRODI] = @Prodi";
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

                        query += " tbl_matakuliah.[NAMA_MK] LIKE @NamaMK";
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

                        query += " tbl_matakuliah.[FAKULTAS] LIKE @Fakultas";
                        parameters.Add("@Fakultas", $"%{fakultas}%");
                    }

                    query += " ORDER BY tbl_matakuliah.[KODE_MK] ASC ";

                    var data = await conn.QueryAsync<TunjanganPengabdianModel>(query, parameters);
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
