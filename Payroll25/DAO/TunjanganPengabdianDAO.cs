using Dapper;
using Humanizer;
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
                        query += "TOP 0 ";
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
                               JOIN PAYROLL.dbo.ref_prodi ON tbl_matakuliah.ID_PRODI = ref_prodi.ID_PRODI
                               JOIN PAYROLL.dbo.ref_fakultas ON ref_fakultas.ID_FAKULTAS = ref_prodi.ID_FAKULTAS";

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

                        query += " ref_fakultas.[FAKULTAS] LIKE @Fakultas";
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

        public bool InsertVakasi(TunjanganPengabdianModel.TunjanganViewModel viewModel)
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
                        ID_KOMPONEN_GAJI = viewModel.TunjanganPengabdian.ID_KOMPONEN_GAJI,
                        ID_BULAN_GAJI = viewModel.TunjanganPengabdian.ID_BULAN_GAJI,
                        NPP = viewModel.TunjanganPengabdian.NPP,
                        JUMLAH = viewModel.TunjanganPengabdian.JUMLAH,
                        DATE_INSERTED = DateTime.Now,
                        DESKRIPSI = viewModel.TunjanganPengabdian.DESKRIPSI
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

        public async Task<IEnumerable<TunjanganPengabdianModel>> GetKomponenGaji(string npp = null)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"SELECT 
                            [ID_KOMPONEN_GAJI]
                            FROM [PAYROLL].[payroll].[TBL_KOMPONEN_GAJI_KARY]
                            WHERE [NPP] = @inputNPP";

                    var parameters = new { inputNPP = npp };

                    var data = await conn.QueryAsync<int>(query, parameters);

                    // Set nilai properti GET_KOMPONEN_GAJI dengan data yang diperoleh dari database
                    var result = data.Select(id => new TunjanganPengabdianModel
                    {
                        GET_KOMPONEN_GAJI = id
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

    }
}
