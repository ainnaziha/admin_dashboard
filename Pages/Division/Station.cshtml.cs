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
        public List<Stesen> listStesen = new List<Stesen>();

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
            OnGetFetchStesen();
        }

        public JsonResult OnGetFetchStesen()
        {
            Debug.WriteLine("Branch OnGetFetchBranch: Fetch branch list");

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
                            Id = reader.GetInt32(0),
                            NamaStesen = reader.GetString(1)
                        };
                        listStesen.Add(stesen);
                    }
                }

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
