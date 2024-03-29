﻿using ClosedXML.Excel;
using CsvHelper.Configuration;
using CsvHelper;
using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Payroll25.DAO
{
    public class IdentitasAsistenDAO
    {
        public async Task<IEnumerable<IdentitasAsistenModel>> ShowIdentitasAssisten(string NPMFilter = null, string NAMAFilter = null, string UNITFilter = null)
        {
            var connectionString = DBkoneksi.payrollkoneksi;

            if (string.IsNullOrEmpty(NPMFilter) && string.IsNullOrEmpty(NAMAFilter) && string.IsNullOrEmpty(UNITFilter))
            {
                return new List<IdentitasAsistenModel>();
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    var baseQuery = @"SELECT DISTINCT
                                    [PAYROLL].[payroll].[TBL_ASISTEN].ID_ASISTEN, 
                                    [PAYROLL].[payroll].[TBL_ASISTEN].ID_TAHUN_AKADEMIK, 
                                    [PAYROLL].[payroll].[TBL_ASISTEN].NO_SEMESTER, 
                                    [PAYROLL].[payroll].[TBL_ASISTEN].NPM,
                                    MHS.nama_mhs AS NAMA_MHS,
                                    [PAYROLL].[payroll].[TBL_ASISTEN].ID_UNIT,
                                    [PAYROLL].[siatmax].[MST_UNIT].NAMA_UNIT,
                                    [PAYROLL].[payroll].[TBL_ASISTEN].NO_REKENING, 
                                    [PAYROLL].[payroll].[TBL_ASISTEN].NAMA_REKENING, 
                                    [PAYROLL].[payroll].[TBL_ASISTEN].NAMA_BANK,
                                    REF.JENIS AS JENIS
                                    FROM 
                                    [PAYROLL].[siatmax].[MST_UNIT]
                                    INNER JOIN 
                                    [PAYROLL].[payroll].[TBL_ASISTEN] 
                                    ON 
                                    [PAYROLL].[siatmax].[MST_UNIT].ID_UNIT = [PAYROLL].[payroll].[TBL_ASISTEN].ID_UNIT
                                    INNER JOIN 
                                    [PAYROLL].[dbo].[mst_mhs_aktif] AS MHS
                                    ON 
                                    [PAYROLL].[payroll].[TBL_ASISTEN].NPM = MHS.npm
                                    INNER JOIN 
                                    [PAYROLL].[payroll].[REF_JENIS_ASISTEN] AS REF
                                    ON 
                                    [PAYROLL].[payroll].[TBL_ASISTEN].ID_JENIS_ASISTEN = REF.ID_JENIS_ASISTEN";

                    var whereClause = "";

                    Dictionary<string, object> parameters = new Dictionary<string, object>();

                    if (!string.IsNullOrEmpty(NPMFilter))
                    {
                        whereClause += " AND TBL_ASISTEN.NPM = @NPMFilter";
                        parameters.Add("@NPMFilter", NPMFilter);
                    }

                    if (!string.IsNullOrEmpty(NAMAFilter))
                    {
                        whereClause += " AND MHS.nama_mhs LIKE @NAMAFilter";
                        parameters.Add("@NAMAFilter", $"%{NAMAFilter}%");
                    }

                    if (!string.IsNullOrEmpty(UNITFilter))
                    {
                        whereClause += " AND MST_UNIT.NAMA_UNIT = @UNITFilter";
                        parameters.Add("@UNITFilter", UNITFilter);
                    }

                    if (!string.IsNullOrEmpty(whereClause))
                    {
                        baseQuery += " WHERE 1=1" + whereClause;
                    }

                    baseQuery += " ORDER BY [PAYROLL].[payroll].[TBL_ASISTEN].ID_ASISTEN DESC;";

                    return await conn.QueryAsync<IdentitasAsistenModel>(baseQuery, parameters);
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


        public IdentitasAsistenModel GetDetails(string npm)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    conn.Open();

                    var query = @"SELECT * FROM payroll.TBL_ASISTEN WHERE NPM = @userNPM";

                    var parameters = new { userNPM = npm };

                    var data = conn.QueryFirstOrDefault<IdentitasAsistenModel>(query, parameters);

                    return data;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public bool InsertDetails(IndexViewModel viewModel)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    conn.Open();

                    var query = @"INSERT INTO payroll.TBL_ASISTEN 
                                (ID_TAHUN_AKADEMIK, NO_SEMESTER, NPM, ID_UNIT, NO_REKENING, NAMA_REKENING, NAMA_BANK,ID_JENIS_ASISTEN) 
                                VALUES 
                                (@ID_Tahun_Akademik, @no_Semester, @npm, @ID_Unit , @no_Rekening, @nama_Rekening, @nama_Bank, @ID_Jenis_Asisten)";

                    var parameters = new
                    {
                        ID_Tahun_Akademik = viewModel.IdentitasAsisten.ID_TAHUN_AKADEMIK,
                        no_Semester = viewModel.IdentitasAsisten.NO_SEMESTER,
                        npm = viewModel.IdentitasAsisten.NPM,
                        ID_Unit = viewModel.IdentitasAsisten.ID_UNIT,
                        no_Rekening = viewModel.IdentitasAsisten.NO_REKENING,
                        nama_Rekening = viewModel.IdentitasAsisten.NAMA_REKENING,
                        nama_Bank = viewModel.IdentitasAsisten.NAMA_BANK,
                        ID_Jenis_Asisten = viewModel.IdentitasAsisten.ID_JENIS_ASISTEN
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

        public bool UpdateDetails(IndexViewModel viewModel,int ID_Asisten)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"UPDATE payroll.TBL_ASISTEN
                                  SET 
                                  ID_TAHUN_AKADEMIK = @ID_Tahun_Akademik, 
                                  NO_SEMESTER = @no_Semester, 
                                  NPM = @npm, 
                                  ID_UNIT = @ID_Unit,
                                  NO_REKENING = @no_Rekening,
                                  NAMA_REKENING = @nama_Rekening,
                                  NAMA_BANK = @nama_Bank,
                                  ID_JENIS_ASISTEN =  @ID_Jenis_Asisten
                                  WHERE ID_ASISTEN = @userID;";

                    var parameters = new
                    {
                        userID = ID_Asisten,
                        ID_Tahun_Akademik = viewModel.IdentitasAsisten.ID_TAHUN_AKADEMIK,
                        no_Semester = viewModel.IdentitasAsisten.NO_SEMESTER,
                        npm = viewModel.IdentitasAsisten.NPM,
                        ID_Unit = viewModel.IdentitasAsisten.ID_UNIT,
                        no_Rekening = viewModel.IdentitasAsisten.NO_REKENING,
                        nama_Rekening = viewModel.IdentitasAsisten.NAMA_REKENING,
                        nama_Bank = viewModel.IdentitasAsisten.NAMA_BANK,
                        ID_Jenis_Asisten = viewModel.IdentitasAsisten.ID_JENIS_ASISTEN
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

        public bool DeleteDetails(IndexViewModel viewModel, int ID_Asisten)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"DELETE FROM payroll.TBL_ASISTEN
                                  WHERE ID_ASISTEN = @userID;";

                    var parameters = new { userID = ID_Asisten };

                    conn.Execute(query, parameters);

                    return true; // Successfully executed the update operation

                }
                catch (Exception)
                {
                    return false; // Failed to execute the update operation
                }
            }
        }

        public IEnumerable<IdentitasAsistenModel> GetUnit()
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

                    var data = conn.Query<IdentitasAsistenModel>(query, parameters).ToList();

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

        public IEnumerable<IdentitasAsistenModel> GetJenisAsisten()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var parameters = new DynamicParameters();

                    var query = @"SELECT 
                                 [ID_JENIS_ASISTEN],
                                 [JENIS]
                                 FROM [PAYROLL].[payroll].[REF_JENIS_ASISTEN];";

                    var data = conn.Query<IdentitasAsistenModel>(query, parameters).ToList();

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

        public (bool, List<string>) UploadAndInsertExcel(IFormFile excelFile)
        {
            var errorMessages = new List<string>();

            try
            {
                using (var stream = excelFile.OpenReadStream())
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheets.First(); // Ambil worksheet pertama

                    var rows = worksheet.RowsUsed().Skip(1); // Skip baris header
                    var records = new List<IdentitasAsistenModel>();

                    foreach (var row in rows)
                    {
                        var record = new IdentitasAsistenModel
                        {
                            ID_TAHUN_AKADEMIK = GetNullableIntValue(row.Cell(1)),
                            NO_SEMESTER = GetNullableIntValue(row.Cell(2)),
                            NPM = GetStringCellValue(row.Cell(3)),
                            ID_UNIT = GetNullableIntValue(row.Cell(4)),
                            NO_REKENING = GetStringCellValue(row.Cell(5)),
                            NAMA_REKENING = GetStringCellValue(row.Cell(6)),
                            NAMA_BANK = GetStringCellValue(row.Cell(7)),
                            ID_JENIS_ASISTEN = GetNullableIntValue(row.Cell(8)),
                        };

                        records.Add(record);
                    }

                    var validRecords = records.Where(record =>
                        record.ID_TAHUN_AKADEMIK != null &&
                        record.NO_SEMESTER != null &&
                        !string.IsNullOrEmpty(record.NPM) &&
                        record.ID_UNIT != null &&
                        !string.IsNullOrEmpty(record.NO_REKENING) &&
                        !string.IsNullOrEmpty(record.NAMA_REKENING) &&
                        !string.IsNullOrEmpty(record.NAMA_BANK) &&
                        record.ID_JENIS_ASISTEN != null
                    ).ToList();

                    var invalidRecords = records.Except(validRecords).ToList();

                    foreach (var invalidRecord in invalidRecords)
                    {
                        errorMessages.Add($"Record dengan NPM {invalidRecord.NPM} memiliki data yang tidak valid atau tidak lengkap.");
                    }

                    using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
                    {
                        conn.Open();
                        var transaction = conn.BeginTransaction();

                        try
                        {
                            foreach (var record in validRecords)
                            {
                                var insertQuery = @"INSERT INTO payroll.TBL_ASISTEN 
                                    (ID_TAHUN_AKADEMIK, NO_SEMESTER, NPM, ID_UNIT, NO_REKENING, NAMA_REKENING, NAMA_BANK, ID_JENIS_ASISTEN) 
                                    VALUES 
                                    (@ID_Tahun_Akademik, @no_Semester, @npm, @ID_Unit , @no_Rekening, @nama_Rekening, @nama_Bank, @ID_Jenis_Asisten)";

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
