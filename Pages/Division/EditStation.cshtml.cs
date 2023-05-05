using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;

namespace spl.Pages.Division
{
    [IgnoreAntiforgeryToken]
    public class EditStationModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public Stesen stesen = new();

        public EditStationModel(IConfiguration config)
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

            FetchStation();
        }

        public void FetchStation()
        {
            Debug.WriteLine("AddSection FetchBranch: Fetch branch list");

            String id = Request.Query["id"];
            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = $"SELECT * FROM stesen WHERE id='{id}'";
                using SqlCommand command = new(sql, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stesen = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            NamaStesen = Convert.ToString(reader["nama_stesen"]) ?? ""
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

        public JsonResult OnPostUpdateStation(Stesen stesen)
        {
            Debug.WriteLine($"UpdateBranch OnPostUpdateBranch: Edit Station {stesen.NamaStesen}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"UPDATE stesen SET nama_stesen='{stesen.NamaStesen}' WHERE id= {stesen.Id};";

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
