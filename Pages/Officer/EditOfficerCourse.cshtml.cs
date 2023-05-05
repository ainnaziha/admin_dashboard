using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace spl.Pages.Officer
{
    [IgnoreAntiforgeryToken]
    public class EditOfficerCourseModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public KursusPegawai kp = new();
        public List<Kursus> list = new();
        public string courseDate = "";
        public string lokasi = "";
        public SelectList? selectKursus { get; set; }

        public EditOfficerCourseModel(IConfiguration config)
        {
            _configuration = config;
        }

        public void OnGet()
        {
            string userType = Request.Cookies["UserType"] ?? "";

            if (userType == "admin")
            {
                Layout = "../Shared/_AdminLayout.cshtml";
            }
            else
            {
                Layout = "../Shared/_UrusetiaLayout.cshtml";
            }

            FetchOfficerCourse();
            FetchCourse();
        }

        public void FetchOfficerCourse()
        {
            Debug.WriteLine("EditOfficerCourse FetchOfficerCourse: Fetch officer");

            String id = Request.Query["id"];

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"SELECT * FROM kursus_pegawai WHERE id='{id}';";

                using SqlCommand command = new(sql, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime tMula = DateTime.Parse(Convert.ToString(reader["tarikh_mula"]) ?? "");
                        DateTime tAkhir = DateTime.Parse(Convert.ToString(reader["tarikh_akhir"]) ?? "");

                        kp = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            TarikhMula = tMula.ToString("yyyy-MM-dd"),
                            TarikhAkhir = tAkhir.ToString("yyyy-MM-dd"),
                            JumlahHari = reader["jumlah_hari"] == DBNull.Value ? 0 : Convert.ToDouble(reader["jumlah_hari"]),
                            IdPegawai = reader["id_pegawai"] == DBNull.Value ? null : Convert.ToInt32(reader["id_pegawai"]),
                            IdKursus = reader["id_kursus"] == DBNull.Value ? null : Convert.ToInt32(reader["id_kursus"]),
                        };
                    }

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditOfficerCourse FetchOfficer Error: {ex.Message}");
            }
        }

        public void FetchCourse()
        {
            Debug.WriteLine("EditOfficerCourse FetchCourse: Fetch course list");

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

                        int? id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]);

                        if (id == kp.IdKursus)
                        {
                            courseDate = $"{tMula:dd/MM/yyyy} - {tAkhir:dd/MM/yyyy}";
                            lokasi = Convert.ToString(reader["lokasi"]) ?? "";
                        }

                        Kursus kursus = new()
                        {
                            Id = id,
                            Tajuk = Convert.ToString(reader["tajuk"]) ?? "",
                            TarikhMula = tMula.ToString("dd/MM/yyyy"),
                            TarikhAkhir = tAkhir.ToString("dd/MM/yyyy"),
                            Lokasi = Convert.ToString(reader["lokasi"]) ?? "",
                        };

                        list.Add(kursus);
                    }

                    selectKursus = new SelectList(list, "Id", "Tajuk", kp.IdKursus);
                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddOfficerCourse FetchCourse Error: {ex.Message}");
            }

        }

        public JsonResult OnPostEditCourse(KursusPegawai kp)
        {
            Debug.WriteLine("EditOfficerCourse OnPostEditCourse: Edit kursus pegawai");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                string dateString = kp.TarikhMula;
                DateTime dateValue = DateTime.Parse(dateString);
                int month = dateValue.Month;

                string sql = $"UPDATE kursus_pegawai SET tarikh_mula='{kp.TarikhMula}', tarikh_akhir='{kp.TarikhAkhir}', " +
                    $"id_kursus={kp.IdKursus}, jumlah_hari={kp.JumlahHari}, bulan={month} " +
                    $"WHERE id= {kp.Id};";

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
