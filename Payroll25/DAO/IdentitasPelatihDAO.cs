using CsvHelper.Configuration;
using CsvHelper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Payroll25.Models;
using System.Data.SqlClient;
using System.Globalization;
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

        public bool DeleteIdentitasPelatih(IdentitasPelatihViewModel viewModel, int ID_Pelatih)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"DELETE FROM payroll.TBL_PELATIH
                                  WHERE ID_PELATIH = @userID;";

                    var parameters = new { userID = ID_Pelatih };

                    conn.Execute(query, parameters);

                    return true; // Successfully executed the update operation

                }
                catch (Exception)
                {
                    return false; // Failed to execute the update operation
                }
            }
        }

        public (bool, List<string>) UploadAndInsertCSV(IFormFile csvFile)
        {
            var errorMessages = new List<string>();
            try
            {
                using (var stream = csvFile.OpenReadStream())
                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    conn.Open();
                    var transaction = conn.BeginTransaction();

                    try
                    {
                        var records = csv.GetRecords<IdentitasPelatihModel>().ToList();
                        var validRecords = records.Where(record =>
                            !string.IsNullOrEmpty(record.NPP) &&
                            !string.IsNullOrEmpty(record.NAMA) &&
                            record.ID_TAHUN_AKADEMIK != null &&
                            record.NO_SEMESTER != null &&
                            record.ID_UNIT != null &&
                            !string.IsNullOrEmpty(record.NO_REKENING) &&
                            !string.IsNullOrEmpty(record.NAMA_REKENING) &&
                            !string.IsNullOrEmpty(record.NAMA_BANK) 
                        ).ToList();

                        var invalidRecords = records.Except(validRecords).ToList();

                        foreach (var invalidRecord in invalidRecords)
                        {
                            errorMessages.Add($"Record dengan NPP {invalidRecord.NPP} memiliki data yang tidak valid atau tidak lengkap.");
                        }

                        foreach (var record in validRecords)
                        {
                            var insertQuery = @"INSERT INTO [payroll].[TBL_PELATIH] 
                                                (NPP, NAMA, ID_TAHUN_AKADEMIK, NO_SEMESTER, ID_UNIT, NO_REKENING, NAMA_REKENING, NAMA_BANK) 
                                                VALUES 
                                                (@NPP, @NAMA, @ID_TAHUN_AKADEMIK, @NO_SEMESTER, @ID_UNIT , @NO_REKENING, @NAMA_REKENING, @NAMA_BANK)";

                            conn.Execute(insertQuery, record, transaction);
                        }

                        transaction.Commit();
                        return (true, errorMessages);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error: " + ex.Message);
                        errorMessages.Add(ex.Message);
                        return (false, errorMessages);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                errorMessages.Add(ex.Message);
                return (false, errorMessages);
            }
        }



    }
}
