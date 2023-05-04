using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using Microsoft.Extensions.Configuration;

namespace spl.Pages.Division
{
    public class AddBranchModel : PageModel
    {
        public String errorMsg = "";
        public String successMsg = "";
        private readonly IConfiguration _configuration;

        public AddBranchModel(IConfiguration config)
        {
            _configuration = config;
        }
        public void OnGet()
        {
        }

        public void OnPost(Bahagian bahagian)
        {
            if (bahagian.NamaBahagian.Length == 0)
            {
                errorMsg = "sila isi nama bahagian!";
                return;
            }

            //save to db
            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "INSERT INTO bahagian " + "(nama_bahagian) VALUES " +
                                  "(@nama_bahagian);";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nama_bahagian", bahagian.NamaBahagian);

                    command.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message.ToString();
                return;
            }
            bahagian.NamaBahagian = "";
            successMsg = "Daftar Berjaya!";

            Response.Redirect("/Division/Branch");
        }
    }
}
