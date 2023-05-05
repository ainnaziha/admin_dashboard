using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;

namespace spl.Pages.Division
{
    [IgnoreAntiforgeryToken]
    public class EditSectionModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public Bahagian bahagian = new();
        public Cawangan cawangan = new();
        public List<Bahagian> listBahagian = new();

        public EditSectionModel(IConfiguration config)
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

            FetchCurrBranch();
            FetchBranch();
            FetchSection();
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
        public void FetchCurrBranch()
        {
            Debug.WriteLine("AddSection FetchBranch: Fetch branch list");

            String id = Request.Query["id"];
            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = $"SELECT * FROM bahagian WHERE id='{id}'";
                using SqlCommand command = new(sql, connection);

                using (SqlDataReader reader = command.ExecuteReader())
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
        public void FetchSection()
        {
            Debug.WriteLine("AddSection FetchBranch: Fetch branch list");

            String id = Request.Query["id"];
            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = $"SELECT * FROM cawangan WHERE id='{id}'";
                using SqlCommand command = new(sql, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cawangan = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            NamaCawangan = Convert.ToString(reader["nama_cawangan"]) ?? ""
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

        public JsonResult OnPostUpdateSection(Cawangan cawangan)
        {
            Debug.WriteLine($"UpdateBranch OnPostUpdateBranch: Edit Branch {cawangan.NamaCawangan}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"UPDATE cawangan SET nama_cawangan='{cawangan.NamaCawangan}', id_bahagian='{cawangan.IdBahagian}' WHERE id= {cawangan.Id};";

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
