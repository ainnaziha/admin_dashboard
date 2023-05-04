using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;

namespace spl.Pages.Division
{
    [IgnoreAntiforgeryToken]
    public class AddGradeModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<Bahagian> listBahagian = new();

        public AddGradeModel(IConfiguration config)
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
        public JsonResult OnPostCreateGrade(Gred gred)
        {
            Debug.WriteLine($"AddBranch OnPostCreateGrade: Adding gred");

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
                
                String sql = $"INSERT INTO gred (abjad, nombor, pangkat, gelaran_pangkat, jabatan, id_kumpulan, nama_kumpulan) " +
                    $"VALUES ('{gred.Abjad}', '{gred.Nombor}', {(gred.Pangkat?.Length > 0 ? $"'{gred.Pangkat}'" : "NULL")}, {(gred.GelaranPangkat?.Length > 0 ? $"'{gred.GelaranPangkat}'" : "NULL")}, '{gred.Jabatan}', {(gred.IdKumpulan != null ? $"{gred.IdKumpulan}" : "NULL")}, {(namaKumpulan != null ? $"'{namaKumpulan}'" : "NULL")});";
                Debug.WriteLine($"AddBranch OnPostCreateGrade: {sql}");

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
