using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;

namespace spl.Pages.Division
{
    [IgnoreAntiforgeryToken]
    public class EditGradeModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public Gred gred = new();

        public EditGradeModel(IConfiguration config)
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
            Debug.WriteLine("AddSection FetchGrade: Fetch grade");

            String id = Request.Query["id"];

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = $"SELECT * FROM gred WHERE id='{id}'";
                using SqlCommand command = new(sql, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        gred = new()
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
                    }

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Edut FetchFrade Error: {ex.Message}");
            }
        }

        public JsonResult OnPostEditGrade(Gred gred)
        {
            Debug.WriteLine($"EditGrade OnPostEditGrade: Edit gred");

            Dictionary<int, string> map = new()
            {
                { 0, "" },
                { 1, "JUSA" },
                { 2, "PENGURUSAN DAN PROFESIONAL" },
                { 3, "PELAKSANA" }
            };

            String? namaKumpulan = map[gred.IdKumpulan ?? 0];
            namaKumpulan = namaKumpulan == "" ? null : namaKumpulan;

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"UPDATE gred SET abjad = '{gred.Abjad}', nombor = '{gred.Nombor}', pangkat = {(gred.Pangkat?.Length > 0 ? $"'{gred.Pangkat}'" : "NULL")}, " +
                    $"gelaran_pangkat = {(gred.GelaranPangkat?.Length > 0 ? $"'{gred.GelaranPangkat}'" : "NULL")}, jabatan = '{gred.Jabatan}', " +
                    $"id_kumpulan = {(gred.IdKumpulan != null ? $"{gred.IdKumpulan}" : "NULL")}, nama_kumpulan = {(namaKumpulan != null ? $"'{namaKumpulan}'" : "NULL")} " +
                    $"WHERE id = {gred.Id};";

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
