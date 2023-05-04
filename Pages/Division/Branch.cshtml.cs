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
        public List<Bahagian> listBahagian = new List<Bahagian>();

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

            OnGetFetchBranch();
        }

        public JsonResult OnGetFetchBranch()
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
                        Bahagian bahagian = new Bahagian();
                        bahagian.Id = reader.GetInt32(0);
                        bahagian.NamaBahagian = reader.GetString(1);
                        listBahagian.Add(bahagian);
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

        public void OnGetFetchSection()
        {
            Debug.WriteLine("Branch OnGetFetchSection: Fetch section list");

        }

        public void OnGetFetchUnit()
        {
            Debug.WriteLine("Branch OnGetFetchUnit: Fetch unit list");

        }
    }
}