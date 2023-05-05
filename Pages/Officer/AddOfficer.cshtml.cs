using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using spl.Model;
using System.Diagnostics;

namespace spl.Pages.Officer
{
    [IgnoreAntiforgeryToken]
    public class AddOfficerModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<Bahagian> listBahagian = new();
        public List<Stesen> listStesen = new();
        public List<Gred> listGred = new();

        public AddOfficerModel(IConfiguration config)
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
            FetchGrade();
            FetchStation();
        }
        public void FetchBranch()
        {
            Debug.WriteLine("AddOfficer FetchBranch: Fetch branch list");
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
                Debug.WriteLine($"AddOfficer FetchBranch Error: {ex.Message}");
            }

        }

        public void FetchGrade()
        {
            Debug.WriteLine("AddOfficer FetchGrade: Fetch grade list");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "SELECT * FROM gred " +
                        "WHERE is_deleted IS NULL OR is_deleted <> 1";

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
                        };

                        listGred.Add(gred);
                    }

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddOfficer FetchGrade Error: {ex.Message}");
            }
        }

        public void FetchStation()
        {
            Debug.WriteLine("AddOfficer FetchStation: Fetch station list");

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
                Debug.WriteLine($"AddOfficer FetchStation Error: {ex.Message}");
            }
        }

        public JsonResult OnPostCawanganUnit(int bahagianId, List<Bahagian> list)
        {
            Debug.WriteLine($"AddOfficer OnPostCawanganUnit: Fetch cawangan and unit list {bahagianId} {list.Count}");

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
                Debug.WriteLine($"AddOfficer OnPostCawanganUnit Error: {ex.Message}");
                return new JsonResult(new { success = false });
            }
        }

        public JsonResult OnPostCreateOfficer(Pegawai pegawai)
        {
            Debug.WriteLine($"AddOfficer OnPostCreateOfficer: Adding pegawai {pegawai.NamaPegawai}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                string sql = $"INSERT INTO pegawai (nama_pegawai, no_ic, id_gred, id_bahagian, id_cawangan, id_unit, id_stesen) " +
                             $"VALUES ('{pegawai.NamaPegawai}', '{pegawai.NoIc}', '{pegawai.IdGred}', '{pegawai.IdBahagian}', {(pegawai.IdCawangan != null ? $"{pegawai.IdCawangan}" : "NULL")}, " +
                             $"{(pegawai.IdUnit != null ? $"{pegawai.IdUnit}" : "NULL")}, {(pegawai.IdStesen != null ? $"{pegawai.IdStesen}" : "NULL")});";

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
