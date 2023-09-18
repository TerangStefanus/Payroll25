using CsvHelper.Configuration;
using CsvHelper;
using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;
using System.Globalization;

namespace Payroll25.DAO
{
    public class PenggajianDAO
    {
        public async Task<IEnumerable<PenggajianModel>> GetKontrakPenggajianDataAsync(string NPPFilter = null, string NAMAFilter = null)
        {
            var connectionString = DBkoneksi.payrollkoneksi;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@NPPFilter", NPPFilter);
                    parameters.Add("@NAMAFilter", NAMAFilter);

                    string query = @"SELECT 
                                    [PAYROLL].[payroll].[TBL_PENGGAJIAN].[ID_PENGGAJIAN],
                                    [PAYROLL].[payroll].[TBL_PENGGAJIAN].[NPP],
                                    [PAYROLL].[payroll].[TBL_PENGGAJIAN].[ID_BULAN_GAJI],
                                    [PAYROLL].[payroll].[TBL_PENGGAJIAN].[NAMA],
                                    [PAYROLL].[payroll].[TBL_PENGGAJIAN].[MASA_KERJA_RIIL],
                                    [PAYROLL].[payroll].[TBL_PENGGAJIAN].[PANGKAT],
                                    [PAYROLL].[payroll].[TBL_PENGGAJIAN].[GOLONGAN],
                                    [PAYROLL].[payroll].[TBL_PENGGAJIAN].[JENJANG],
                                    [PAYROLL].[payroll].[TBL_PENGGAJIAN].[NO_TABUNGAN]
                                    FROM 
                                        [PAYROLL].[payroll].[TBL_PENGGAJIAN]
                                    INNER JOIN 
                                        [PAYROLL].[simka].[MST_KARYAWAN]
                                    ON 
                                        [PAYROLL].[payroll].[TBL_PENGGAJIAN].[NPP] = [PAYROLL].[simka].[MST_KARYAWAN].[NPP]
                                    WHERE 
                                        [PAYROLL].[simka].[MST_KARYAWAN].[STATUS_KEPEGAWAIAN] = 'Kontrak' 
                                        AND (@NPPFilter IS NULL OR [PAYROLL].[payroll].[TBL_PENGGAJIAN].[NPP] LIKE '%' + @NPPFilter + '%')
                                        AND (@NAMAFilter IS NULL OR [PAYROLL].[payroll].[TBL_PENGGAJIAN].[NAMA] LIKE '%' + @NAMAFilter + '%')";

                    if (string.IsNullOrEmpty(NPPFilter) && string.IsNullOrEmpty(NAMAFilter))
                    {
                        return new List<PenggajianModel>();
                    }

                    return await conn.QueryAsync<PenggajianModel>(query, parameters);
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


        public int InsertKontrakPenggajianData(PenggajianModel model)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"INSERT INTO  [PAYROLL].[payroll].[TBL_PENGGAJIAN]
                                ([NPP],[ID_BULAN_GAJI],[NAMA],[STATUS_KEPEGAWAIAN],[MASA_KERJA_RIIL],[MASA_KERJA_GOL],[JBT_STRUKTURAL],
                                [JBT_AKADEMIK],[JBT_FUNGSIONAL],[PANGKAT],[GOLONGAN],[JENJANG],[NO_TABUNGAN],[NPWP])
                                VALUES
                                (@NPP,@ID_BULAN_GAJI,@NAMA,@STATUS_KEPEGAWAIAN,@MASA_KERJA_RIIL,@MASA_KERJA_GOL,@JBT_STRUKTURAL,
                                @JBT_AKADEMIK,@JBT_FUNGSIONAL,@PANGKAT,@GOLONGAN,@JENJANG,@NO_TABUNGAN,@NPWP)";

                    var parameters = new
                    {
                        NPP = model.NPP,
                        ID_BULAN_GAJI = model.ID_BULAN_GAJI,
                        NAMA = model.NAMA,
                        STATUS_KEPEGAWAIAN = model.STATUS_KEPEGAWAIAN,
                        MASA_KERJA_RIIL = model.MASA_KERJA_RIIL,
                        MASA_KERJA_GOL = model.MASA_KERJA_GOL,
                        JBT_STRUKTURAL = model.JBT_STRUKTURAL,
                        JBT_AKADEMIK = model.JBT_AKADEMIK,
                        JBT_FUNGSIONAL = model.JBT_FUNGSIONAL,
                        PANGKAT = model.PANGKAT,
                        GOLONGAN = model.GOLONGAN,
                        JENJANG = model.JENJANG,
                        NO_TABUNGAN = model.NO_TABUNGAN,
                        NPWP = model.NPWP,
                    };

                    return conn.Execute(query, parameters);
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

        public int UpdateKontrakPenggajianData(List<PenggajianModel> model)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"UPDATE [payroll].[TBL_PENGGAJIAN]
                                SET
                                [NPP] = @NPP,
                                [NAMA] = @NAMA,
                                [PANGKAT] = @PANGKAT,
                                [GOLONGAN] = @GOLONGAN,
                                [JENJANG] = @JENJANG,
                                [NO_TABUNGAN] = @NO_TABUNGAN,
                                [ID_BULAN_GAJI] = @ID_BULAN_GAJI
                                WHERE ID_PENGGAJIAN = @ID_PENGGAJIAN";

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

        public int DeleteKontrakPenggajianData(List<PenggajianModel> model)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"DELETE FROM [payroll].[TBL_PENGGAJIAN]
                                WHERE ID_PENGGAJIAN = @ID_PENGGAJIAN";

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


        public int InsertDetailPenggajian(PenggajianModel model)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"INSERT INTO  [PAYROLL].[payroll].[DTL_PENGGAJIAN]
                                ([ID_PENGGAJIAN],[ID_KOMPONEN_GAJI],[JUMLAH_SATUAN],[NOMINAL])
                                VALUES
                                (@ID_PENGGAJIAN,@ID_KOMPONEN_GAJI,@JUMLAH_SATUAN,@NOMINAL)";

                    var parameters = new
                    {
                        ID_PENGGAJIAN = model.ID_PENGGAJIAN,
                        ID_KOMPONEN_GAJI = model.ID_KOMPONEN_GAJI,
                        JUMLAH_SATUAN = model.JUMLAH_SATUAN,
                        NOMINAL = model.NOMINAL
                    };

                    return conn.Execute(query, parameters);
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





        public async Task<IEnumerable<PenggajianModel>> GetPenggajianDataAsync(string NPPFilter = null)
        {
            var connectionString = DBkoneksi.payrollkoneksi;

            if (string.IsNullOrEmpty(NPPFilter))
            {
                return new List<PenggajianModel>();
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = @"
                    DECLARE @NPP VARCHAR(50);
                    SET @NPP = @NppParam;

                    SELECT 
                    TBL_BEBAN_MENGAJAR.NPP, 
	                MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI,
                    MST_KOMPONEN_GAJI.KOMPONEN_GAJI AS NAMA_KOMPONEN_GAJI, 
                    TBL_BEBAN_MENGAJAR.TOTAL_SKS AS JUMLAH, 
                    MST_TARIF_PAYROLL.NOMINAL AS TARIF
                    FROM [PAYROLL].[payroll].[TBL_BEBAN_MENGAJAR]
                    INNER JOIN [PAYROLL].[simka].[MST_KARYAWAN] ON TBL_BEBAN_MENGAJAR.NPP = MST_KARYAWAN.NPP
                    JOIN [PAYROLL].[simka].[MST_TARIF_PAYROLL] ON MST_KARYAWAN.ID_REF_JBTN_AKADEMIK = MST_TARIF_PAYROLL.ID_REF_JBTN_AKADEMIK 
                    JOIN [PAYROLL].[payroll].[MST_KOMPONEN_GAJI] ON MST_TARIF_PAYROLL.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI 
                    JOIN [PAYROLL].[simka].[REF_JENJANG] ON MST_KARYAWAN.PENDIDIKAN_TERAKHIR = REF_JENJANG.DESKRIPSI AND REF_JENJANG.ID_REF_JENJANG = MST_TARIF_PAYROLL.ID_REF_JENJANG
                    WHERE 
                        MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI = 77
                        AND TBL_BEBAN_MENGAJAR.NPP = @NPP

                    UNION ALL 

                    SELECT DISTINCT
                    MST_KARYAWAN.NPP,
	                MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI,
                    MST_KOMPONEN_GAJI.KOMPONEN_GAJI AS NAMA_KOMPONEN_GAJI, 
                    TBL_VAKASI.JUMLAH, 
                    MST_TARIF_PAYROLL.NOMINAL AS TARIF
                    FROM PAYROLL.simka.MST_KARYAWAN
                    JOIN PAYROLL.payroll.TBL_VAKASI ON MST_KARYAWAN.NPP = TBL_VAKASI.NPP
                    JOIN PAYROLL.payroll.MST_KOMPONEN_GAJI ON TBL_VAKASI.ID_KOMPONEN_GAJI = MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI
                    JOIN PAYROLL.simka.MST_TARIF_PAYROLL ON MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI = MST_TARIF_PAYROLL.ID_KOMPONEN_GAJI
                    WHERE 
                        MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI BETWEEN 78 AND 201 
                        AND MST_KARYAWAN.NPP = @NPP";

                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@NppParam", NPPFilter }
                    };

                    return await conn.QueryAsync<PenggajianModel>(query, parameters);
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
                        var records = csv.GetRecords<PenggajianModel>().ToList();
                        var validRecords = records.Where(record =>
                            record.ID_PENGGAJIAN != null &&
                            record.ID_KOMPONEN_GAJI != null &&
                            record.JUMLAH_SATUAN != null &&
                            record.NOMINAL != null                             
                        ).ToList();

                        var invalidRecords = records.Except(validRecords).ToList();

                        foreach (var invalidRecord in invalidRecords)
                        {
                            errorMessages.Add($"Record dengan ID PENGGAJIAN {invalidRecord.ID_PENGGAJIAN} memiliki data yang tidak valid atau tidak lengkap.");
                        }

                        foreach (var record in validRecords)
                        {
                            var insertQuery = @"INSERT INTO [PAYROLL].[payroll].[DTL_PENGGAJIAN]
                                                ([ID_PENGGAJIAN],[ID_KOMPONEN_GAJI],[JUMLAH_SATUAN],[NOMINAL])
                                                VALUES
                                                (@ID_PENGGAJIAN,@ID_KOMPONEN_GAJI,@JUMLAH_SATUAN,@NOMINAL)";

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


        // Support untuk Dropdown Insert ID_BULAN_GAJI
        public async Task<IEnumerable<PenggajianModel>> GetBulanGaji(int tahun = 0)
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
                    var result = data.Select(id_bulan => new PenggajianModel
                    {
                        GET_BULAN_GAJI = id_bulan
                    });

                    return result.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<PenggajianModel>();
                }
            }
        }

        // Support untuk Dropdown Insert Komponen Gaji
        public async Task<IEnumerable<PenggajianModel>> GetKomponenGaji()
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var query = @"SELECT 
                                [ID_KOMPONEN_GAJI],
                                [KOMPONEN_GAJI]
                                FROM [PAYROLL].[payroll].[MST_KOMPONEN_GAJI]
                                WHERE MST_KOMPONEN_GAJI.ID_KOMPONEN_GAJI BETWEEN 77 AND 201 ";

                    var data = await conn.QueryAsync<PenggajianModel>(query);

                    return data.ToList();
                }
                catch (Exception)
                {
                    // Handle exceptions here
                    return Enumerable.Empty<PenggajianModel>();
                }
            }
        }



    }
}
