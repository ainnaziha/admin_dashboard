using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using spl.Model;
using System.Diagnostics;
using Npgsql;

namespace spl.Pages.Division
{
    [IgnoreAntiforgeryToken]
    public class EditBranchModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public Bahagian bahagian = new();

        public EditBranchModel(IConfiguration config)
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

            FetchBranch();
        }

        public void FetchBranch()
        {
            Debug.WriteLine("EditBranch FetchBranch: Fetch branch list");

            String id = Request.Query["id"];
            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                connection.Open();
                String sql = $"SELECT * FROM bahagian WHERE id='{id}'";
                using NpgsqlCommand command = new NpgsqlCommand(sql, connection);

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bahagian = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            NamaBahagian = Convert.ToString(reader["nama_bahagian"]) ?? ""
                        };
                    }

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddUnit FetchBranch Error: {ex.Message}");
            }
        }
        public JsonResult OnPostUpdateBranch(Bahagian bahagian)
        {
            Debug.WriteLine($"UpdateBranch OnPostUpdateBranch: Edit Branch {bahagian.NamaBahagian}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                connection.Open();

                String sql = $"UPDATE bahagian SET nama_bahagian='{bahagian.NamaBahagian}' WHERE id= {bahagian.Id};";

                using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
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
