﻿using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class HonorPendadaranDAO
    {
        public async Task<IEnumerable<HonorPendadaranModel>> ShowHonorPendadaranAsync()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var parameters = new DynamicParameters();

                    var query = @"SELECT
                                TBL_VAKASI.ID_VAKASI,
                                MST_KARYAWAN.NPP,
                                MST_KARYAWAN.NAMA,
                                MST_KOMPONEN_GAJI.KOMPONEN_GAJI AS KEGIATAN,
	                            TBL_VAKASI.JUMLAH AS Jml_Mhs,
                                CONVERT(varchar, TBL_VAKASI.DATE_INSERTED, 101) AS Tgl_buat
                                FROM 
                                PAYROLL.simka.MST_KARYAWAN
                                JOIN 
                                PAYROLL.payroll.TBL_VAKASI ON MST_KARYAWAN.NPP = TBL_VAKASI.NPP
                                JOIN 
                                PAYROLL.payroll.MST_KOMPONEN_GAJI ON TBL_VAKASI.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI
                                WHERE 
                                MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI = 76";

                    var data = conn.Query<HonorPendadaranModel>(query, parameters).ToList();

                    return data;
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return null;
                }
            }
        }

        public bool InsertVakasi(HonorPendadaranModel.HonorPendadaranViewModel viewModel)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    conn.Open();

                    var query = @"INSERT INTO payroll.TBL_VAKASI
                                  (ID_KOMPONEN_GAJI, ID_BULAN_GAJI,NPP, JUMLAH, DATE_INSERTED,DESKRIPSI) 
                                  VALUES 
                                  (@ID_KOMPONEN_GAJI, @ID_BULAN_GAJI, @NPP, @JUMLAH,@DATE_INSERTED,@DESKRIPSI)";

                    var parameters = new
                    {
                        ID_KOMPONEN_GAJI = viewModel.HonorPendadaran.ID_KOMPONEN_GAJI,
                        ID_BULAN_GAJI = viewModel.HonorPendadaran.ID_BULAN_GAJI,
                        NPP = viewModel.HonorPendadaran.NPP,
                        JUMLAH = viewModel.HonorPendadaran.JUMLAH,
                        DATE_INSERTED = DateTime.Now,
                        DESKRIPSI = viewModel.HonorPendadaran.DESKRIPSI
                    };

                    conn.Execute(query, parameters);

                    return true; // Berhasil melakukan insert
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return false; // Gagal melakukan insert
                }
            }
        }

        public async Task<IEnumerable<TunjanganPengabdianModel>> GetBulanGaji(int tahun = 0)
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
                    var result = data.Select(id_bulan => new TunjanganPengabdianModel
                    {
                        GET_BULAN_GAJI = id_bulan
                    });

                    return result.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<TunjanganPengabdianModel>();
                }
            }
        }

        public bool UpdateVakasiHonor(HonorPendadaranModel.HonorPendadaranViewModel viewModel, int ID_Vakasi)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"UPDATE payroll.TBL_VAKASI
                                 SET 
                                 ID_KOMPONEN_GAJI = @ID_KOMPONEN_GAJI,
                                 ID_BULAN_GAJI = @ID_BULAN_GAJI, 
                                 JUMLAH = @JUMLAH, 
                                 DATE_INSERTED = @DATE_INSERTED, 
                                 DESKRIPSI = @DESKRIPSI
                                 WHERE ID_VAKASI = @userID;";

                    var parameters = new
                    {
                        userID = ID_Vakasi,
                        ID_KOMPONEN_GAJI = viewModel.HonorPendadaran.ID_KOMPONEN_GAJI,
                        ID_BULAN_GAJI = viewModel.HonorPendadaran.ID_BULAN_GAJI,
                        JUMLAH = viewModel.HonorPendadaran.JUMLAH,
                        DATE_INSERTED = DateTime.Now,
                        DESKRIPSI = viewModel.HonorPendadaran.DESKRIPSI
                    };

                    conn.Execute(query, parameters);

                    return true; // Successfully executed the update operation

                }
                catch (Exception)
                {
                    return false; // Failed to execute the update operation
                }
            }
        }

        public bool DeleteVakasiHonor(HonorPendadaranModel.HonorPendadaranViewModel viewModel, int ID_Vakasi)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"DELETE FROM payroll.TBL_VAKASI
                                  WHERE ID_VAKASI = @userID;";

                    var parameters = new { userID = ID_Vakasi };

                    conn.Execute(query, parameters);

                    return true; // Successfully executed the update operation

                }
                catch (Exception)
                {
                    return false; // Failed to execute the update operation
                }
            }
        }



    }
}
