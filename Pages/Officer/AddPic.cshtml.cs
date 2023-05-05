using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using spl.Middleware;

namespace spl.Pages.Officer
{
    [IgnoreAntiforgeryToken]
    public class AddPicMode : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<Bahagian> listBahagian = new();
        public List<Stesen> listStesen = new();

        public AddPicMode(IConfiguration config)
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

            FetchBranch();
            FetchStation();
        }
        public void FetchBranch()
        {
            Debug.WriteLine("AddPic FetchBranch: Fetch branch list");
            Bahagian currentBahagian = new();

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = "SELECT b.id AS id_bahagian, b.nama_bahagian, c.id AS id_cawangan, c.nama_cawangan, u.id AS id_unit, u.nama_unit " +
                        "FROM bahagian b " +
                        "LEFT JOIN cawangan c ON c.id_bahagian = b.id " +
                        "LEFT JOIN unit u ON u.id_bahagian = b.id " +
                        "WHERE (b.is_deleted IS NULL OR b.is_deleted <> 1) AND (c.is_deleted IS NULL OR c.is_deleted <> 1) AND (u.is_deleted IS NULL OR u.is_deleted <> 1)";


                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int bahagianId = Convert.ToInt32(reader["id_bahagian"]);

                        if (currentBahagian == null || currentBahagian.Id != bahagianId)
                        {
                            currentBahagian = new Bahagian
                            {
                                Id = bahagianId,
                                NamaBahagian = Convert.ToString(reader["nama_bahagian"]) ?? "",
                                Cawangans = new List<Cawangan>(),
                                Units = new List<Unit>()
                            };

                            listBahagian.Add(currentBahagian);
                        }

                        if (currentBahagian != null)
                        {
                            if (reader["id_cawangan"] != DBNull.Value)
                            {
                                currentBahagian.Cawangans.Add(new Cawangan
                                {
                                    Id = Convert.ToInt32(reader["id_cawangan"]),
                                    NamaCawangan = Convert.ToString(reader["nama_cawangan"]) ?? ""
                                });
                            }

                            if (reader["id_unit"] != DBNull.Value)
                            {
                                // add the current unit to the list of units for the current bahagian
                                currentBahagian.Units.Add(new Unit
                                {
                                    Id = Convert.ToInt32(reader["id_unit"]),
                                    NamaUnit = Convert.ToString(reader["nama_unit"]) ?? ""
                                });
                            }
                        }
                    }

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddPic FetchBranch Error: {ex.Message}");
            }

        }

        public void FetchStation()
        {
            Debug.WriteLine("AddPic FetchStation: Fetch station list");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "SELECT * FROM stesen " + 
                    "WHERE is_deleted IS NULL OR is_deleted <> 1";

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
                Debug.WriteLine($"AddPic FetchStation Error: {ex.Message}");
            }
        }

        public JsonResult OnPostCawanganUnit(int bahagianId, List<Bahagian> list)
        {
            Debug.WriteLine($"AddPic OnPostCawanganUnit: Fetch cawangan and unit list {bahagianId} {list.Count}");

            try
            {
                Bahagian? bahagian = list.FirstOrDefault(b => b.Id == bahagianId);

                if (bahagian == null)
                {
                    return new JsonResult(new { success = false, msg="Bahagian does not exist" });
                }

                List<Cawangan> cawanganList = new();
                if (bahagian.Cawangans != null)
                {
                    foreach (Cawangan c in bahagian.Cawangans)
                    {
                        Cawangan newCawangan = new()
                        {
                            Id = c.Id,
                            NamaCawangan = c.NamaCawangan
                        };
                        cawanganList.Add(newCawangan);
                    }
                }

                List<Unit> unitList = new();
                if (bahagian.Units != null)
                {
                    foreach (Unit u in bahagian.Units)
                    {
                        Unit newUnit = new()
                        {
                            Id = u.Id,
                            NamaUnit = u.NamaUnit
                        };
                        unitList.Add(newUnit);
                    }
                }

                return new JsonResult(new { success = true, cawangan = cawanganList, unit = unitList });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddPic OnPostCawanganUnit Error: {ex.Message}");
                return new JsonResult(new { success = false });
            }
        }

        public JsonResult OnPostCreatePic(User user)
        {
            Debug.WriteLine($"AddPic OnPostCreatePic: Adding urusetia {user.Username}");

            String password = CryptoMiddleWare.ComputeSHA256Hash(user.Password);

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                string sql = $"INSERT INTO users (username, password, id_bahagian, id_cawangan, id_unit, id_stesen, user_type) " +
                             $"VALUES ('{user.Username}', '{password}', {user.IdBahagian}, {(user.IdCawangan != null ? $"{user.IdCawangan}" : "NULL")}, " +
                             $"{(user.IdUnit != null ? $"{user.IdUnit}" : "NULL")}, {user.IdStesen}, 'urusetia');";

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
