using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Data.SqlClient;
using spl.Model;
using Microsoft.AspNetCore.Mvc;

namespace spl.Pages.Division
{
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
            Debug.WriteLine("Branch OnGetFetchBranch: Fetch branch list");

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
                            Id = reader.GetInt32(0),
                            NamaBahagian = reader.GetString(1)
                        };
                        listBahagian.Add(bahagian);
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Branch OnGetFetchBranch Error: {ex.Message}");
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
                String sql = "SELECT c.id, c.nama_cawangan, b.id, b.nama_bahagian " +
                    "FROM cawangan c " +
                    "JOIN bahagian b ON c.id_bahagian = b.id;";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cawangan cawangan = new()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            NamaCawangan = Convert.ToString(reader["nama_cawangan"]) ?? "",
                            Bahagian = new Bahagian()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                NamaBahagian = Convert.ToString(reader["nama_bahagian"]) ?? "",
                            }
                        };

                        list.Add(cawangan);
                    }
                }

                connection.Close();
                return new JsonResult(new { success = true, data = list });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, msg = ex.Message });
            }

        }

        public void OnGetFetchUnit()
        {
            Debug.WriteLine("Branch OnGetFetchUnit: Fetch unit list");

        }
    }
}