using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;
using static Payroll25.Models.IdentitasDosenModel;

namespace Payroll25.DAO
{
    public class IdentitasDosenDAO
    {
        public IEnumerable<IdentitasDosenModel> ShowIdentitasDosen()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var parameters = new DynamicParameters();
                    // Jika memiliki parameter dalam query dapat ditambahkan ke objek DynamicParameters.
                    
                    var query = @"SELECT 
                                simka.MST_KARYAWAN.NPP, 
                                simka.MST_KARYAWAN.NAMA,
                                CONVERT(DATE, simka.MST_KARYAWAN.TGL_LAHIR) as TGL_LAHIR,
                                simka.MST_KARYAWAN.JNS_KEL, 
                                simka.MST_KARYAWAN.ID_UNIT, 
                                simka.MST_KARYAWAN.ID_REF_GOLONGAN, 
                                simka.MST_KARYAWAN.NO_TELPON_HP, 
                                simka.MST_KARYAWAN.NPWP, 
                                simka.MST_KARYAWAN.ALAMAT,
                                [PAYROLL].[simka].[MST_REKENING].NO_REKENING,
                                [PAYROLL].[simka].[MST_REKENING].NAMA_REKENING,
                                [PAYROLL].[simka].[MST_REKENING].NAMA_BANK,
	                            [PAYROLL].[simka].[MST_REKENING].STATUS_REKENING		
                                FROM simka.MST_KARYAWAN
                                INNER JOIN [PAYROLL].[simka].[MST_REKENING]
                                    ON simka.MST_KARYAWAN.NPP = [PAYROLL].[simka].[MST_REKENING].NPP
                                WHERE simka.MST_KARYAWAN.STATUS_KEPEGAWAIAN = 'Kontrak'";

                    var data = conn.Query<IdentitasDosenModel>(query, parameters).ToList();

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

        public bool InsertDetails(IdentitasDosenViewModel viewModel)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    conn.Open();

                    var query = @"INSERT INTO simka.MST_REKENING
                                (NO_REKENING, NPP, NAMA_BANK, STATUS_REKENING ,STATUS , NAMA_REKENING) 
                                VALUES 
                                (@no_Rekening, @npp, @nama_Bank, @status_Rekening , @status, @nama_Rekening)";

                    var parameters = new
                    {
                        no_Rekening = viewModel.IdentitasDosen.NO_REKENING,
                        npp = viewModel.IdentitasDosen.NPP,
                        nama_Bank = viewModel.IdentitasDosen.NAMA_BANK,
                        status_Rekening = viewModel.IdentitasDosen.STATUS_REKENING,
                        status = viewModel.IdentitasDosen.STATUS,
                        nama_Rekening = viewModel.IdentitasDosen.NAMA_REKENING
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

        //Get NPP untuk keperluan input dan dropdown
        public IEnumerable<IdentitasDosenModel> GetNPP()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var parameters = new DynamicParameters();

                    var query = @"SELECT [NPP]
                                FROM [PAYROLL].[simka].[MST_KARYAWAN]
                                WHERE simka.MST_KARYAWAN.STATUS_KEPEGAWAIAN = 'Kontrak';";

                    var data = conn.Query<IdentitasDosenModel>(query, parameters).ToList();

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

        public bool UpdateDetails(IdentitasDosenViewModel viewModel, string NO_Rekening)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"UPDATE simka.MST_REKENING
                                  SET 
                                  NO_REKENING = @no_Rekening, 
                                  NPP = @npp, 
                                  NAMA_BANK = @nama_Bank, 
                                  STATUS_REKENING = @status_Rekening,
                                  STATUS = @status,
                                  NAMA_REKENING = @nama_Rekening
                                  WHERE NO_REKENING = @userNO;";

                    var parameters = new
                    {
                        userNO = NO_Rekening,
                        no_Rekening = viewModel.IdentitasDosen.NO_REKENING,
                        npp = viewModel.IdentitasDosen.NPP,
                        nama_Bank = viewModel.IdentitasDosen.NAMA_BANK,
                        status_Rekening = viewModel.IdentitasDosen.STATUS_REKENING,
                        status = viewModel.IdentitasDosen.STATUS,
                        nama_Rekening = viewModel.IdentitasDosen.NAMA_REKENING
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

        public bool DeleteDetails(IdentitasDosenViewModel viewModel, string NO_Rekening)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"DELETE FROM simka.MST_REKENING
                                  WHERE NO_REKENING = @userNO;";

                    var parameters = new { userNO = NO_Rekening };

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
