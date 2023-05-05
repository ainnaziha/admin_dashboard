using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Data.SqlClient;
using spl.Model;
using Microsoft.AspNetCore.Mvc;

namespace spl.Pages.Division
{
    [IgnoreAntiforgeryToken]
    public class BranchModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<Bahagian> listBahagian = new();

        public BranchModel(IConfiguration config)
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
            Debug.WriteLine("Branch FetchBranch: Fetch branch list");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "SELECT * FROM bahagian " +
                        "WHERE is_deleted IS NULL OR is_deleted <> 1;";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
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

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Branch FetchBranch Error: {ex.Message}");
            }
        }

        public JsonResult OnGetFetchSection()
        {
            Debug.WriteLine("Branch OnGetFetchSection: Fetch section list");

            List<Cawangan> list = new();

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "SELECT c.id, c.nama_cawangan, b.id as id_bahagian, b.nama_bahagian " +
                    "FROM cawangan c " +
                    "JOIN bahagian b ON c.id_bahagian = b.id " +
                    "WHERE c.is_deleted IS NULL OR c.is_deleted <> 1;";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cawangan cawangan = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            NamaCawangan = Convert.ToString(reader["nama_cawangan"]) ?? "",
                            Bahagian = new Bahagian()
                            {
                                Id = reader["id_bahagian"] == DBNull.Value ? null : Convert.ToInt32(reader["id_bahagian"]),
                                NamaBahagian = Convert.ToString(reader["nama_bahagian"]) ?? "",
                            }
                        };

                        list.Add(cawangan);
                    }

                    reader.Close();
                }

                connection.Close();
                return new JsonResult(new { success = true, data = list });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, msg = ex.Message });
            }

        }

        public JsonResult OnGetFetchUnit()
        {
            Debug.WriteLine("Branch OnGetFetchUnit: Fetch unit list");

            List<Unit> list = new();

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "SELECT c.id, c.nama_unit, b.id as id_bahagian, b.nama_bahagian " +
                    "FROM unit c " +
                    "JOIN bahagian b ON c.id_bahagian = b.id " +
                    "WHERE c.is_deleted IS NULL OR c.is_deleted <> 1;";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Unit unit = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            NamaUnit = Convert.ToString(reader["nama_unit"]) ?? "",
                            Bahagian = new Bahagian()
                            {
                                Id = reader["id_bahagian"] == DBNull.Value ? null : Convert.ToInt32(reader["id_bahagian"]),
                                NamaBahagian = Convert.ToString(reader["nama_bahagian"]) ?? "",
                            }
                        };

                        list.Add(unit);
                    }

                    reader.Close();
                }

                connection.Close();
                return new JsonResult(new { success = true, data = list });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, msg = ex.Message });
            }

        }

        public JsonResult OnPostDeleteBranch(int id)
        {
            Debug.WriteLine($"Branch OnPostDeleteBranch: Delete item {id}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"UPDATE bahagian SET is_deleted = 1 WHERE id = {id};";

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
        public JsonResult OnPostDeleteSection(int id)
        {
            Debug.WriteLine($"Branch OnPostDeleteSection: Delete item {id}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"UPDATE cawangan SET is_deleted = 1 WHERE id = {id};";

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

        public JsonResult OnPostDeleteUnit(int id)
        {
            Debug.WriteLine($"Branch OnPostDeleteUnit: Delete item {id}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"UPDATE unit SET is_deleted = 1 WHERE id = {id};";

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