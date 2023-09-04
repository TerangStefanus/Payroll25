using Dapper;
using Microsoft.AspNetCore.Mvc;
using Payroll25.Models;
using System.Data.SqlClient;
using static Payroll25.Models.IdentitasPelatihModel;

namespace Payroll25.DAO
{
    public class IdentitasPelatihDAO
    {
        public IEnumerable<IdentitasPelatihModel> ShowIdentitasPelatih()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var parameters = new DynamicParameters();

                    var query = @"SELECT
                                [PAYROLL].[payroll].[TBL_PELATIH].[ID_PELATIH],
                                [PAYROLL].[payroll].[TBL_PELATIH].[NPP],
                                [PAYROLL].[payroll].[TBL_PELATIH].[NAMA],
                                [PAYROLL].[payroll].[TBL_PELATIH].[ID_TAHUN_AKADEMIK],
                                [PAYROLL].[payroll].[TBL_PELATIH].[NO_SEMESTER],
	                            [PAYROLL].[siatmax].[MST_UNIT].[ID_UNIT],
	                            [PAYROLL].[siatmax].[MST_UNIT].[NAMA_UNIT],
                                [PAYROLL].[payroll].[TBL_PELATIH].[NO_REKENING],
                                [PAYROLL].[payroll].[TBL_PELATIH].[NAMA_REKENING],
                                [PAYROLL].[payroll].[TBL_PELATIH].[NAMA_BANK]
                                FROM [PAYROLL].[payroll].[TBL_PELATIH]
                                INNER JOIN [PAYROLL].[siatmax].[MST_UNIT] ON [PAYROLL].[payroll].[TBL_PELATIH].[ID_UNIT] = [PAYROLL].[siatmax].[MST_UNIT].[ID_UNIT]";

                    var data = conn.Query<IdentitasPelatihModel>(query, parameters).ToList();

                    return data;
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

        public bool InsertIdentitasPelatih(IdentitasPelatihViewModel viewModel)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    conn.Open();

                    var query = @"INSERT INTO [payroll].[TBL_PELATIH] 
                                (NPP, NAMA, ID_TAHUN_AKADEMIK, NO_SEMESTER, ID_UNIT, NO_REKENING, NAMA_REKENING, NAMA_BANK) 
                                VALUES 
                                (@NPP, @NAMA, @ID_TAHUN_AKADEMIK, @NO_SEMESTER, @ID_UNIT , @NO_REKENING, @NAMA_REKENING, @NAMA_BANK)";

                    var parameters = new
                    {
                        NPP = viewModel.IdentitasPelatih.NPP,
                        NAMA = viewModel.IdentitasPelatih.NAMA,
                        ID_TAHUN_AKADEMIK = viewModel.IdentitasPelatih.ID_TAHUN_AKADEMIK,
                        NO_SEMESTER = viewModel.IdentitasPelatih.NO_SEMESTER,
                        ID_UNIT = viewModel.IdentitasPelatih.ID_UNIT,
                        NO_REKENING = viewModel.IdentitasPelatih.NO_REKENING,
                        NAMA_REKENING = viewModel.IdentitasPelatih.NAMA_REKENING,
                        NAMA_BANK = viewModel.IdentitasPelatih.NAMA_BANK,
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

        public IEnumerable<IdentitasPelatihModel> GetUnit()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var parameters = new DynamicParameters();

                    var query = @"SELECT 
                                 [ID_UNIT], 
                                 [NAMA_UNIT] 
                                 FROM [PAYROLL].[siatmax].[MST_UNIT];";

                    var data = conn.Query<IdentitasPelatihModel>(query, parameters).ToList();

                    return data;
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

        public bool UpdateIdentitasPelatih(IdentitasPelatihViewModel viewModel, int ID_Pelatih)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"UPDATE payroll.TBL_PELATIH
                                  SET
                                  NPP = @NPP,
                                  NAMA = @NAMA,
                                  ID_TAHUN_AKADEMIK = @ID_TAHUN_AKADEMIK, 
                                  NO_SEMESTER = @NO_SEMESTER,  
                                  ID_UNIT = @ID_UNIT,
                                  NO_REKENING = @NO_REKENING,
                                  NAMA_REKENING = @NAMA_REKENING,
                                  NAMA_BANK = @NAMA_BANK
                                  WHERE ID_PELATIH = @userID;";

                    var parameters = new
                    {
                        userID = ID_Pelatih,
                        NPP = viewModel.IdentitasPelatih.NPP,
                        NAMA = viewModel.IdentitasPelatih.NAMA,
                        ID_TAHUN_AKADEMIK = viewModel.IdentitasPelatih.ID_TAHUN_AKADEMIK,
                        NO_SEMESTER = viewModel.IdentitasPelatih.NO_SEMESTER,
                        ID_UNIT = viewModel.IdentitasPelatih.ID_UNIT,
                        NO_REKENING = viewModel.IdentitasPelatih.NO_REKENING,
                        NAMA_REKENING = viewModel.IdentitasPelatih.NAMA_REKENING,
                        NAMA_BANK = viewModel.IdentitasPelatih.NAMA_BANK,
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

        


    }
}
