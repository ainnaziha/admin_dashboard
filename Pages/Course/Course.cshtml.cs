using Microsoft.AspNetCore.Mvc.RazorPages;
using spl.Model;
using System.Data.SqlClient;
using System.Diagnostics;

namespace spl.Pages.Course
{
    public class CourseModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<Kursus> listKursus = new();
        public CourseModel(IConfiguration config)
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

            FetchCourse();
        }

        public void FetchCourse()
        {
            Debug.WriteLine("Course FetchCourse: Fetch course list");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = "SELECT k.id, k.tajuk, k.lokasi, k.tarikh_mula, k.tarikh_akhir, kk.id as id_kategori, kk.nama_kategori " +
                    "FROM kursus k " +
                    "LEFT JOIN kategori_kursus kk ON k.id_kategori = kk.id;";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Kursus kursus = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            Tajuk = Convert.ToString(reader["tajuk"]) ?? "",
                            Lokasi = Convert.ToString(reader["lokasi"]) ?? "",
                            TarikhMula = Convert.ToString(reader["tarikh_mula"]) ?? "",
                            TarikhAkhir = Convert.ToString(reader["tarikh_akhir"]) ?? "",
                            KategoriKursus = reader["id_kategori"] == DBNull.Value ? null : new KategoriKursus() {
                                Id = reader["id_kategori"] == DBNull.Value ? null : Convert.ToInt32(reader["id_kategori"]),
                                NamaKategori = Convert.ToString(reader["nama_kategori"]) ?? "",
                            }
                        };

                        listKursus.Add(kursus);
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Course FetchCourse Error: {ex.Message}");
            }
        }
    }
}
