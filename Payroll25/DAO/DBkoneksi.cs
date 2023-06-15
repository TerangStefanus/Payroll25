namespace Payroll25.DAO
{
    public class DBkoneksi
    {
        // internet publik
        //private static string IPdb = "202.14.92.208";

        //internet lokal
        private static string IPdb = "192.168.15.156";

        public static string payrollkoneksi = $"Server={IPdb};Database=PAYROLL;User Id=payroll25;Password=payroll25123!";
    }
}
