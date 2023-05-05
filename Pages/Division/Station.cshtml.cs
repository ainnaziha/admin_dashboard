using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Data.SqlClient;
using spl.Model;
using Microsoft.AspNetCore.Mvc;

namespace spl.Pages.Division
{
    public class StationModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<Stesen> listStesen = new();

        public StationModel(IConfiguration config)
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
            Debug.WriteLine("Station FetchStation: Fetch station list");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "SELECT * FROM stesen";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Stesen stesen = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            NamaStesen = Convert.ToString(reader["nama_stesen"]) ?? "",
                        };

                        listStesen.Add(stesen);
                    }

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Station FetchStation Error: {ex.Message}");
            }
        }
    }
}
