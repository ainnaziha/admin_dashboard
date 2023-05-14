using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace spl.Pages.Officer
{
    [IgnoreAntiforgeryToken]
    public class EditPicModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public User user = new();
        public List<Bahagian> listBahagian = new();
        public List<Stesen> listStesen = new();
        public List<Cawangan> listCawangan = new();
        public List<Unit> listUnit = new();
        public SelectList? selectBahagian { get; set; }
        public SelectList? selectStesen { get; set; }
        public SelectList? selectCawangan { get; set; }
        public SelectList? selectUnit { get; set; }
        public EditPicModel(IConfiguration config)
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

            FetchUser();
            FetchBranch();
            FetchStation();
            return Page();
        }

        public void FetchUser()
        {
            Debug.WriteLine("EditPic FetchUser: Fetch user");

            String id = Request.Query["id"];
            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = $"SELECT * FROM users WHERE id='{id}'";
                using SqlCommand command = new(sql, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            Username = Convert.ToString(reader["username"]) ?? "",
                            IdBahagian = reader["id_bahagian"] == DBNull.Value ? null : Convert.ToInt32(reader["id_bahagian"]),
                            IdCawangan = reader["id_cawangan"] == DBNull.Value ? null : Convert.ToInt32(reader["id_cawangan"]),
                            IdUnit = reader["id_unit"] == DBNull.Value ? null : Convert.ToInt32(reader["id_unit"]),
                            IdStesen = reader["id_stesen"] == DBNull.Value ? null : Convert.ToInt32(reader["id_stesen"]),
                        };
                    }

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditPic FetchUser Error: {ex.Message}");
            }
        }
        public void FetchBranch()
        {
            Debug.WriteLine("EditPic FetchBranch: Fetch branch list");
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
                                int cawanganId = Convert.ToInt32(reader["id_cawangan"]);
                                bool cawanganExists = currentBahagian.Cawangans.Any(c => c.Id == cawanganId);

                                if (!cawanganExists)
                                {
                                    currentBahagian.Cawangans.Add(new Cawangan
                                    {
                                        Id = cawanganId,
                                        NamaCawangan = Convert.ToString(reader["nama_cawangan"]) ?? ""
                                    });
                                }
                            }

                            if (reader["id_unit"] != DBNull.Value)
                            {
                                int unitId = Convert.ToInt32(reader["id_unit"]);
                                bool unitExists = currentBahagian.Units.Any(u => u.Id == unitId);

                                if (!unitExists)
                                {
                                    currentBahagian.Units.Add(new Unit
                                    {
                                        Id = unitId,
                                        NamaUnit = Convert.ToString(reader["nama_unit"]) ?? ""
                                    });
                                }
                            }
                        }
                    }

                    selectBahagian = new SelectList(listBahagian, "Id", "NamaBahagian", user.IdBahagian);

                    FetchCawanganUnit();
                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditPic FetchBranch Error: {ex.Message}");
            }

        }

        public void FetchCawanganUnit()
        {
            Debug.WriteLine($"EditPic FetchCawanganUnit: Fetch cawangan and unit list");

            try
            {
                Bahagian? bahagian = listBahagian.FirstOrDefault(b => b.Id == user.IdBahagian);

                if (bahagian != null)
                {
                    if (bahagian.Cawangans != null)
                    {
                        foreach (Cawangan c in bahagian.Cawangans)
                        {
                            Cawangan newCawangan = new()
                            {
                                Id = c.Id,
                                NamaCawangan = c.NamaCawangan
                            };
                            listCawangan.Add(newCawangan);
                        }
                    }

                    if (bahagian.Units != null)
                    {
                        foreach (Unit u in bahagian.Units)
                        {
                            Unit newUnit = new()
                            {
                                Id = u.Id,
                                NamaUnit = u.NamaUnit
                            };
                            listUnit.Add(newUnit);
                        }
                    }

                    selectCawangan = new SelectList(listCawangan, "Id", "NamaCawangan", user.IdCawangan);
                    selectUnit = new SelectList(listUnit, "Id", "NamaUnit", user.IdUnit);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditPic FetchCawanganUnit Error: {ex.Message}");
            }
        }

        public void FetchStation()
        {
            Debug.WriteLine("EditPic FetchStation: Fetch station list");

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
                    selectStesen = new SelectList(listStesen, "Id", "NamaStesen", user.IdStesen);

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditPic FetchStation Error: {ex.Message}");
            }
        }
        public JsonResult OnPostCawanganUnit(int bahagianId, List<Bahagian> list)
        {
            Debug.WriteLine($"EditPic OnPostCawanganUnit: Fetch cawangan and unit list {bahagianId} {list.Count}");

            try
            {
                Bahagian? bahagian = list.FirstOrDefault(b => b.Id == bahagianId);

                if (bahagian == null)
                {
                    return new JsonResult(new { success = false, msg = "Bahagian does not exist" });
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
                Debug.WriteLine($"EditPic OnPostCawanganUnit Error: {ex.Message}");
                return new JsonResult(new { success = false });
            }
        }
        public JsonResult OnPostEditUser(User user)
        {
            Debug.WriteLine($"EditPic OnPostEditOfficer: Edit pegawai {user.Username}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                string sql = $"UPDATE users SET username='{user.Username}', " +
                    $"id_bahagian={user.IdBahagian}, id_cawangan={(user.IdCawangan != null ? $"{user.IdCawangan}" : "NULL")}, " +
                    $"id_unit={(user.IdUnit != null ? $"{user.IdUnit}" : "NULL")}, id_stesen={(user.IdStesen != null ? $"{user.IdStesen}" : "NULL")} " +
                    $"WHERE id= {user.Id};";

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
