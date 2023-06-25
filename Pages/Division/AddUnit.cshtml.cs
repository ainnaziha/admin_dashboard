using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using spl.Model;
using System.Diagnostics;
using Npgsql;

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
                using NpgsqlConnection connection = new NpgsqlConnection(connectionString);                
                connection.Open();
                String sql = "SELECT * FROM bahagian " +
                        "WHERE is_deleted IS NULL OR is_deleted <> 1;";

                using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                using (NpgsqlDataReader reader = command.ExecuteReader())
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

                    reader.Close();
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
                using NpgsqlConnection connection = new NpgsqlConnection(connectionString);                
                connection.Open();

                String sql = $"INSERT INTO unit (nama_unit, id_bahagian) VALUES ('{unit.NamaUnit}', {unit.IdBahagian});";

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
