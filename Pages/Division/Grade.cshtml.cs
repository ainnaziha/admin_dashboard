using Microsoft.AspNetCore.Mvc.RazorPages;
using spl.Model;
using System.Data.SqlClient;
using System.Diagnostics;

namespace spl.Pages.Division
{
    public class GradeModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<Gred> listGred = new();
        public GradeModel(IConfiguration config)
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

            FetchGrade();
        }

        public void FetchGrade()
        {
            Debug.WriteLine("Station FetchGrade: Fetch grade list");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "SELECT * FROM gred";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Gred gred = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            Abjad = Convert.ToString(reader["abjad"]) ?? "",
                            Nombor = Convert.ToString(reader["nombor"]) ?? "",
                            Pangkat = reader["pangkat"] == DBNull.Value ? null : Convert.ToString(reader["pangkat"]),
                            GelaranPangkat = reader["gelaran_pangkat"] == DBNull.Value ? null : Convert.ToString(reader["gelaran_pangkat"]),
                            Jabatan = Convert.ToString(reader["jabatan"]) ?? "",
                            IdKumpulan = reader["id_kumpulan"] == DBNull.Value ? null : Convert.ToInt32(reader["id_kumpulan"]),
                            NamaKumpulan = reader["nama_kumpulan"] == DBNull.Value ? null : Convert.ToString(reader["nama_kumpulan"])
                        };

                        listGred.Add(gred);
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Branch FetchGrade Error: {ex.Message}");
            }

        }
    }
}
