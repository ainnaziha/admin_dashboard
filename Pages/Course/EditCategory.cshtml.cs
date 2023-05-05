using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;

namespace spl.Pages.Course
{
    [IgnoreAntiforgeryToken]
    public class EditCategoryModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public KategoriKursus kategori = new();

        public EditCategoryModel(IConfiguration config)
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
            Debug.WriteLine("EditCategory FetchCategory: Fetch grade");

            String id = Request.Query["id"];

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"SELECT * FROM kategori_kursus WHERE id='{id}';";

                using SqlCommand command = new(sql, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kategori = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            NamaKategori = Convert.ToString(reader["nama_kategori"]) ?? "",
                        };
                    }

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditCategory FetchFrade Error: {ex.Message}");
            }
        }

        public JsonResult OnPostEditCategory(KategoriKursus kategori)
        {
            Debug.WriteLine($"EditCategory OnPostEditCategory: Edit kategori {kategori.NamaKategori}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"UPDATE kategori_kursus SET nama_kategori = '{kategori.NamaKategori}' WHERE id = {kategori.Id};";

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
