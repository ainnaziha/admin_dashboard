using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;
using System.Globalization;

namespace spl.Pages.Course
{
    [IgnoreAntiforgeryToken]
    public class AddCourseModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<KategoriKursus> listKategori = new();

        public AddCourseModel(IConfiguration config)
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

            FetchCategory();
        }
        public void FetchCategory()
        {
            Debug.WriteLine("AddCourse FetchCategory: Fetch category list");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "SELECT * FROM kategori_kursus " +
                    "WHERE is_deleted IS NULL OR is_deleted <> 1;";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        KategoriKursus kategori = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            NamaKategori = Convert.ToString(reader["nama_kategori"]) ?? "",
                        };

                        listKategori.Add(kategori);
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddCourse FetchCategory Error: {ex.Message}");
            }

        }

        public JsonResult OnPostCreateCourse(Kursus kursus)
        {
            Debug.WriteLine($"AddCourse OnPostCreateCourse: Adding kursus {kursus.Tajuk} {kursus.TarikhMula}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                string sql = $"INSERT INTO kursus (tajuk, tarikh_mula, tarikh_akhir, lokasi, id_kategori) " +
                             $"VALUES ('{kursus.Tajuk}', '{kursus.TarikhMula}', '{kursus.TarikhAkhir}', '{kursus.Lokasi}', {(kursus.IdKategori != null ? $"{kursus.IdKategori}" : "NULL")});";


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
