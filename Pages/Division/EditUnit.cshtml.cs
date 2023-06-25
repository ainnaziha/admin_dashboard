using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using spl.Model;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql;


namespace spl.Pages.Division
{
    [IgnoreAntiforgeryToken]

    public class EditUnitModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public Unit unit = new();
        public List<Bahagian> listBahagian = new();
       public SelectList? selectBahagian { get; set; }

        public EditUnitModel(IConfiguration config)
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
            FetchUnit();
            FetchBranch();
        }
        public void FetchUnit()
        {
            Debug.WriteLine("AddSection FetchBranch: Fetch branch list");

            String id = Request.Query["id"];
            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                connection.Open();
                String sql = $"SELECT * FROM unit WHERE id='{id}'";
                using NpgsqlCommand command = new NpgsqlCommand(sql, connection);

                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        unit = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            NamaUnit = Convert.ToString(reader["nama_unit"]) ?? "",
                            IdBahagian = reader["id_bahagian"] == DBNull.Value ? null : Convert.ToInt32(reader["id_bahagian"]),
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
        public void FetchBranch()
        {
            Debug.WriteLine("Branch FetchBranch: Fetch branch list");

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
                            NamaBahagian = Convert.ToString(reader["nama_bahagian"]) ?? "",
                        };
                        listBahagian.Add(bahagian);
                    }
                    selectBahagian = new SelectList(listBahagian, "Id", "NamaBahagian", unit.IdBahagian);
                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Branch FetchBranch Error: {ex.Message}");
            }
        }
       
        public JsonResult OnPostUpdateUnit(Unit unit)
        {
            Debug.WriteLine($"UpdateBranch OnPostUpdateBranch: Edit Branch {unit.NamaUnit}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                connection.Open();

                String sql = $"UPDATE unit SET nama_unit='{unit.NamaUnit}', id_bahagian='{unit.IdBahagian}' WHERE id= {unit.Id};";

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
