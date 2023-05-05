using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace spl.Pages.Course
{
    [IgnoreAntiforgeryToken]
    public class EditCourseModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<KategoriKursus> listKategori = new();
        public Kursus kursus = new();
        public SelectList? selectKategori { get; set; }

        public EditCourseModel(IConfiguration config)
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
            FetchCategory();
        }

        public void FetchCourse()
        {
            Debug.WriteLine("EditCourse FetchCourse: Fetch course list");

            String id = Request.Query["id"];

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"SELECT * FROM kursus WHERE id='{id}'";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime tMula = DateTime.Parse(Convert.ToString(reader["tarikh_mula"]) ?? "");
                        DateTime tAkhir = DateTime.Parse(Convert.ToString(reader["tarikh_akhir"]) ?? "");

                        kursus = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            Tajuk = Convert.ToString(reader["tajuk"]) ?? "",
                            Lokasi = Convert.ToString(reader["lokasi"]) ?? "",
                            TarikhMula = tMula.ToString("yyyy-MM-dd"),
                            TarikhAkhir = tAkhir.ToString("yyyy-MM-dd"),
                            IdKategori = reader["id_kategori"] == DBNull.Value ? null : Convert.ToInt32(reader["id_kategori"]),
                        };

                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditCourse FetchCourse Error: {ex.Message}");
            }
        }

        public void FetchCategory()
        {
            Debug.WriteLine("EditCourse FetchCategory: Fetch category list");

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

                    selectKategori = new SelectList(listKategori, "Id", "NamaKategori", kursus.IdKategori);
                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditCourse FetchCategory Error: {ex.Message}");
            }

        }

        public JsonResult OnPostEditCourse(Kursus kursus)
        {
            Debug.WriteLine($"EditCourse OnPostEditCourse: Edit kursus {kursus.Tajuk} {kursus.TarikhMula}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                string sql = $"UPDATE kursus SET tajuk='{kursus.Tajuk}', tarikh_mula='{kursus.TarikhMula}', tarikh_akhir='{kursus.TarikhAkhir}', " +
                    $"lokasi='{kursus.Lokasi}', id_kategori={(kursus.IdKategori != null ? $"{kursus.IdKategori}" : "NULL")} " +
                    $" WHERE id= {kursus.Id};";

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
