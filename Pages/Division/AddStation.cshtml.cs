using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;

namespace spl.Pages.Division
{
    [IgnoreAntiforgeryToken]
    public class AddStationModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }

        public AddStationModel(IConfiguration config)
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
        }

        public JsonResult OnPostCreateStation(Stesen stesen)
        {
            Debug.WriteLine($"AddStation OnPostCreateStation: Adding stesen {stesen.NamaStesen}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "INSERT INTO stesen " + "(nama_stesen) VALUES " +
                                                 "(@nama_stesen);";

                using SqlCommand command = new(sql, connection);

                command.Parameters.AddWithValue("@nama_stesen", stesen.NamaStesen);
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
