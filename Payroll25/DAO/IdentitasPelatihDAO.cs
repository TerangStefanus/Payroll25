using ClosedXML.Excel;
using CsvHelper.Configuration;
using CsvHelper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Payroll25.Models;
using System.Data.SqlClient;
using System.Globalization;
using static Payroll25.Models.IdentitasPelatihModel;
using System.Security.Cryptography;
using System.Text;

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
                                (NPP, NAMA, ID_TAHUN_AKADEMIK, NO_SEMESTER, ID_UNIT, NO_REKENING, NAMA_REKENING, NAMA_BANK, TGL_LAHIR, PASSWORD, ALAMAT, NO_TELP) 
                                VALUES 
                                (@NPP, @NAMA, @ID_TAHUN_AKADEMIK, @NO_SEMESTER, @ID_UNIT , @NO_REKENING, @NAMA_REKENING, @NAMA_BANK, @TGL_LAHIR, @PASSWORD, @ALAMAT, @NO_TELP)";

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
                        TGL_LAHIR = new DateTime(1999, 09, 09).ToString("yyyy-MM-dd"),
                        PASSWORD = _encPass("1234567"),
                        ALAMAT = "Alamat NPP " + viewModel.IdentitasPelatih.NPP,
                        NO_TELP = "08XXXXXXXXXX"
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

        private string _encPass(string password)
        {
            // password to RIPEMD160
            string hash = "";
            Encoding enc = Encoding.GetEncoding(1252);
            RIPEMD160 ripemdHasher = RIPEMD160.Create();
            byte[] data = ripemdHasher.ComputeHash(Encoding.Default.GetBytes(password));
            hash = enc.GetString(data);

            return hash;
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

        public (bool, List<string>) UploadAndInsertExcel(IFormFile excelFile)
        {
            var errorMessages = new List<string>();

            try
            {
                using (var stream = excelFile.OpenReadStream())
                using (var workbook = new XLWorkbook(stream))
                using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                {
                    conn.Open();
                    var transaction = conn.BeginTransaction();

                    try
                    {
                        var worksheet = workbook.Worksheets.First(); // Ambil worksheet pertama

                        var rows = worksheet.RowsUsed().Skip(1); // Skip baris header
                        var records = new List<IdentitasPelatihModel>();

                        foreach (var row in rows)
                        {
                            var record = new IdentitasPelatihModel
                            {
                                NPP = GetStringCellValue(row.Cell(1)),
                                NAMA = GetStringCellValue(row.Cell(2)),
                                ID_TAHUN_AKADEMIK = GetNullableIntValue(row.Cell(3)),
                                NO_SEMESTER = GetNullableIntValue(row.Cell(4)),
                                ID_UNIT = GetNullableIntValue(row.Cell(5)),
                                NO_REKENING = GetStringCellValue(row.Cell(6)),
                                NAMA_REKENING = GetStringCellValue(row.Cell(7)),
                                NAMA_BANK = GetStringCellValue(row.Cell(8)),
                            };

                            records.Add(record);
                        }

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
                            record.ALAMAT = "Alamat NPP " + record.NPP;
                            record.NO_TELP = "08XXXXXXXXXX";
                            record.PASSWORD = _encPass("1234567");
                            record.TGL_LAHIR = DateTime.ParseExact("1999-09-09", "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            var insertQuery = @"INSERT INTO [payroll].[TBL_PELATIH] 
                                               (NPP, NAMA, ID_TAHUN_AKADEMIK, NO_SEMESTER, ID_UNIT, NO_REKENING, NAMA_REKENING, NAMA_BANK, PASSWORD, TGL_LAHIR) 
                                               VALUES 
                                               (@NPP, @NAMA, @ID_TAHUN_AKADEMIK, @NO_SEMESTER, @ID_UNIT , @NO_REKENING, @NAMA_REKENING, @NAMA_BANK, @PASSWORD, @TGL_LAHIR)";

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

        private int? GetNullableIntValue(IXLCell cell)
        {
            if (cell.IsEmpty())
            {
                return null;
            }

            int result;
            if (int.TryParse(cell.Value.ToString(), out result))
            {
                return result;
            }

            return null;
        }

        private string GetStringCellValue(IXLCell cell)
        {
            return cell.IsEmpty() ? null : cell.Value.ToString();
        }


    }
}
