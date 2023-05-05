using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;
using System.Globalization;

namespace spl.Pages.Officer
{
    [IgnoreAntiforgeryToken]
    public class AddOfficerCourseModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<Kursus> list = new();
        public string pegawaiId = "";

        public AddOfficerCourseModel(IConfiguration config)
        {
            _configuration = config;
        }

        public void OnGet()
        {
            pegawaiId = Request.Query["id"];
            string userType = Request.Cookies["UserType"] ?? "";

            if (userType == "admin")
            {
                Layout = "../Shared/_AdminLayout.cshtml";
            }
            else
            {
                Layout = "../Shared/_UrusetiaLayout.cshtml";
            }

            FetchCourse();
        }
        public void FetchCourse()
        {
            Debug.WriteLine("AddOfficerCourse FetchCourse: Fetch course list");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "SELECT * FROM kursus " +
                    "WHERE is_deleted IS NULL OR is_deleted <> 1;";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime tMula = DateTime.Parse(Convert.ToString(reader["tarikh_mula"]) ?? "");
                        DateTime tAkhir = DateTime.Parse(Convert.ToString(reader["tarikh_akhir"]) ?? "");

                        Kursus kursus = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            Tajuk = Convert.ToString(reader["tajuk"]) ?? "",
                            TarikhMula = tMula.ToString("dd/MM/yyyy"),
                            TarikhAkhir = tAkhir.ToString("dd/MM/yyyy"),
                            Lokasi = Convert.ToString(reader["lokasi"]) ?? "",
                        };

                        list.Add(kursus);
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddOfficerCourse FetchCourse Error: {ex.Message}");
            }

        }

        public JsonResult OnPostCreateCourse(KursusPegawai kp)
        {
            Debug.WriteLine("AddOfficerCourse OnPostCreateCourse: Adding kursus pegawai");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                string dateString = kp.TarikhMula;
                DateTime dateValue = DateTime.Parse(dateString);
                int month = dateValue.Month;

                string sql = "INSERT INTO kursus_pegawai (tarikh_mula, tarikh_akhir, id_kursus, id_pegawai, jumlah_hari, bulan) " +
                             $"VALUES ('{kp.TarikhMula}', '{kp.TarikhAkhir}', {kp.IdKursus}, {kp.IdPegawai}, {kp.JumlahHari}, {month});";

                using SqlCommand command = new(sql, connection);
                command.ExecuteNonQuery();

                connection.Close();

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, msg = ex.Message });
            }
        }
    }
}
