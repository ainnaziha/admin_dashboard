using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;

namespace spl.Pages.Course
{
    [IgnoreAntiforgeryToken]
    public class AddCategoryModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }

        public AddCategoryModel(IConfiguration config)
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
        }
        public JsonResult OnPostCreateCategory(KategoriKursus kategori)
        {
            Debug.WriteLine($"Addcategory OnPostCreateCategory: Adding kategori");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                
                String sql = $"INSERT INTO kategori_kursus (nama_kategori) " +
                    $"VALUES ('{kategori.NamaKategori}');";
                Debug.WriteLine($"AddCategory OnPostCreateGrade: {sql}");

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
