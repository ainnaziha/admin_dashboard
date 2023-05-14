using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using spl.Model;
using System.Data.SqlClient;
using System.Diagnostics;

namespace spl.Pages.Officer
{
    [IgnoreAntiforgeryToken]
    public class PicModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<User> listUser = new();
        public PicModel(IConfiguration config)
        {
            _configuration = config;
        }

        public IActionResult OnGet()
        {
            string userType = Request.Cookies["UserType"] ?? "";

            if (userType == "admin")
            {
                Layout = "../Shared/_AdminLayout.cshtml";
            }
            else
            {
                return RedirectToPage("/notfound");
            }

            FetchPic();
            return Page();
        }

        public void FetchPic()
        {
            Debug.WriteLine("Pic FetchPic: Fetch pic list");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = "SELECT p.id, p.username, " +
                    "p.id_bahagian, b.nama_bahagian, p.id_cawangan, c.nama_cawangan, p.id_unit, u.nama_unit, p.id_stesen, s.nama_stesen " +
                    "FROM users p " +
                    "LEFT JOIN bahagian b ON p.id_bahagian = b.id " +
                    "LEFT JOIN cawangan c ON p.id_cawangan = c.id " +
                    "LEFT JOIN unit u ON p.id_unit = u.id " +
                    "LEFT JOIN stesen s ON p.id_stesen = s.id " +
                    "WHERE p.user_type = 'urusetia' " +
                    "AND (p.is_deleted IS NULL OR p.is_deleted <> 1);";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            Username = Convert.ToString(reader["username"]) ?? "",
                            Bahagian = reader["id_bahagian"] == DBNull.Value ? null : new Bahagian()
                            {
                                Id = reader["id_bahagian"] == DBNull.Value ? null : Convert.ToInt32(reader["id_bahagian"]),
                                NamaBahagian = Convert.ToString(reader["nama_bahagian"]) ?? "",
                            },
                            Cawangan = reader["id_cawangan"] == DBNull.Value ? null : new Cawangan()
                            {
                                Id = reader["id_cawangan"] == DBNull.Value ? null : Convert.ToInt32(reader["id_cawangan"]),
                                NamaCawangan = Convert.ToString(reader["nama_cawangan"]) ?? "",
                            },
                            Unit = reader["id_unit"] == DBNull.Value ? null : new Unit()
                            {
                                Id = reader["id_unit"] == DBNull.Value ? null : Convert.ToInt32(reader["id_unit"]),
                                NamaUnit = Convert.ToString(reader["nama_unit"]) ?? "",
                            },
                            Stesen = reader["id_stesen"] == DBNull.Value ? null : new Stesen()
                            {
                                Id = reader["id_stesen"] == DBNull.Value ? null : Convert.ToInt32(reader["id_stesen"]),
                                NamaStesen = Convert.ToString(reader["nama_stesen"]) ?? "",
                            },
                        };

                        listUser.Add(user);
                    }

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Pic FetchPic Error: {ex.Message}");
            }
        }

        public JsonResult OnPostDelete(int id)
        {
            Debug.WriteLine($"Pic OnPostDelete: Delete item {id}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"UPDATE users SET is_deleted = 1 WHERE id = {id};";

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
