using Dapper;
using Payroll25.Models;
using System.Data.SqlClient;

namespace Payroll25.DAO
{
    public class IdentitasAsistenDAO
    {
        public IEnumerable<IdentitasAsistenModel> ShowIdentitasAssisten()
        {
            using(SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    var parameters = new DynamicParameters();

                    var query = @"SELECT * FROM payroll.TBL_ASISTEN";

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

        public IdentitasAsistenModel GetDetails(string npm)
        {
            using (SqlConnection conn = new SqlConnection(DBkoneksi.payrollkoneksi))
            {
                try
                {
                    conn.Open();

                    var query = @"SELECT * 
                          FROM payroll.TBL_ASISTEN 
                          WHERE NPM = @userNPM";

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
                                (ID_TAHUN_AKADEMIK, NO_SEMESTER, NPM, ID_UNIT) 
                                VALUES 
                                (@ID_Tahun_Akademik, @no_Semester, @npm, @ID_Unit)";

                    var parameters = new
                    {
                        ID_Tahun_Akademik = viewModel.IdentitasAsisten.ID_TAHUN_AKADEMIK,
                        no_Semester = viewModel.IdentitasAsisten.NO_SEMESTER,
                        npm = viewModel.IdentitasAsisten.NPM,
                        ID_Unit = viewModel.IdentitasAsisten.ID_UNIT
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



    }
}
