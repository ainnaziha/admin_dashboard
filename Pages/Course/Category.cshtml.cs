using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using spl.Model;
using System.Data.SqlClient;
using System.Diagnostics;

namespace spl.Pages.Course
{
    [IgnoreAntiforgeryToken]
    public class CategoryModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<KategoriKursus> listKategori = new();
        public CategoryModel(IConfiguration config)
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
            Debug.WriteLine("Category FetchCategory: Fetch category list");

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

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Category FetchCategory Error: {ex.Message}");
            }

        }

        public JsonResult OnPostDelete(int id)
        {
            Debug.WriteLine($"Category OnPostDelete: Delete item {id}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"UPDATE kategori_kursus SET is_deleted = 1 WHERE id = {id};";

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
