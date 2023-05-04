using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;

namespace spl.Pages.Division
{
    [IgnoreAntiforgeryToken]
    public class AddBranchModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }

        public AddBranchModel(IConfiguration config)
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

        public JsonResult OnPostCreateBranch(Bahagian bahagian)
        {
            Debug.WriteLine($"AddBranch OnPostCreateBranch: Adding bahagian {bahagian.NamaBahagian}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = $"INSERT INTO bahagian (nama_bahagian) VALUES ('{bahagian.NamaBahagian}');";

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
