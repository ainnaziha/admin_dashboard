using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;

namespace spl.Pages.Division
{
    [IgnoreAntiforgeryToken]
    public class AddUnitModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<Bahagian> listBahagian = new();

        public AddUnitModel(IConfiguration config)
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
            Debug.WriteLine("AddSection FetchBranch: Fetch branch list");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "SELECT * FROM bahagian";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Bahagian bahagian = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            NamaBahagian = Convert.ToString(reader["nama_bahagian"]) ?? ""
                        };

                        listBahagian.Add(bahagian);
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddUnit FetchBranch Error: {ex.Message}");
            }
        }

        public JsonResult OnPostCreateUnit(Unit unit)
        {
            Debug.WriteLine($"AddUnit OnPostCreatUnit: Adding unit {unit.NamaUnit}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"INSERT INTO unit (nama_unit, id_bahagian) VALUES ('{unit.NamaUnit}', {unit.IdBahagian});";

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
