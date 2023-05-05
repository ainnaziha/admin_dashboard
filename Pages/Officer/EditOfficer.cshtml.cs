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
    public class EditOfficerModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public Pegawai pegawai = new();
        public List<Bahagian> listBahagian = new();
        public List<Stesen> listStesen = new();
        public List<Gred> listGred = new();
        public List<Cawangan> listCawangan = new();
        public List<Unit> listUnit = new();
        public SelectList? selectBahagian { get; set; }
        public SelectList? selectStesen { get; set; }
        public SelectList? selectGred { get; set; }
        public SelectList? selectCawangan { get; set; }
        public SelectList? selectUnit { get; set; }


        public EditOfficerModel(IConfiguration config)
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

            FetchOfficer();
            FetchBranch();
            FetchGrade();
            FetchStation();
        }

        public void FetchOfficer()
        {
            Debug.WriteLine("EditOfficer FetchOfficer: Fetch officer");

            String id = Request.Query["id"];

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"SELECT * FROM pegawai WHERE id='{id}';";

                using SqlCommand command = new(sql, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pegawai = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            NamaPegawai = Convert.ToString(reader["nama_pegawai"]) ?? "",
                            NoIc = Convert.ToString(reader["no_ic"]) ?? "",
                            IdGred = reader["id_gred"] == DBNull.Value ? null : Convert.ToInt32(reader["id_gred"]),
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
                Debug.WriteLine($"EditOfficer FetchOfficer Error: {ex.Message}");
            }
        }

        public void FetchBranch()
        {
            Debug.WriteLine("EditOfficer FetchBranch: Fetch branch list");
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

                    selectBahagian = new SelectList(listBahagian, "Id", "NamaBahagian", pegawai.IdBahagian);

                    FetchCawanganUnit();
                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditOfficer FetchBranch Error: {ex.Message}");
            }

        }

        public void FetchCawanganUnit()
        {
            Debug.WriteLine($"EditOfficer FetchCawanganUnit: Fetch cawangan and unit list");

            try
            {
                Bahagian? bahagian = listBahagian.FirstOrDefault(b => b.Id == pegawai.IdBahagian);

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

                    selectCawangan = new SelectList(listCawangan, "Id", "NamaCawangan", pegawai.IdCawangan);
                    selectUnit = new SelectList(listUnit, "Id", "NamaUnit", pegawai.IdUnit);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditOfficer FetchCawanganUnit Error: {ex.Message}");
            }
        }

        public void FetchGrade()
        {
            Debug.WriteLine("EditOfficer FetchGrade: Fetch grade list");

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

                    selectGred = new SelectList(listGred.Select(item => new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = $"{item.Abjad} {item.Nombor} {(item.GelaranPangkat != null ? $"({item.GelaranPangkat})" : "")}"
                    }).ToList(), "Value", "Text", pegawai.IdGred);
                    
                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditOfficer FetchGrade Error: {ex.Message}");
            }
        }

        public void FetchStation()
        {
            Debug.WriteLine("EditOfficer FetchStation: Fetch station list");

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

                    selectStesen = new SelectList(listStesen, "Id", "NamaStesen", pegawai.IdStesen);
                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditOfficer FetchStation Error: {ex.Message}");
            }
        }

        public JsonResult OnPostCawanganUnit(int bahagianId, List<Bahagian> list)
        {
            Debug.WriteLine($"EditOfficer OnPostCawanganUnit: Fetch cawangan and unit list {bahagianId} {list.Count}");

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
                Debug.WriteLine($"EditOfficer OnPostCawanganUnit Error: {ex.Message}");
                return new JsonResult(new { success = false });
            }
        }

        public JsonResult OnPostEditOfficer(Pegawai pegawai)
        {
            Debug.WriteLine($"EditOfficer OnPostEditOfficer: Edit pegawai {pegawai.NamaPegawai}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                string sql = $"UPDATE pegawai SET nama_pegawai='{pegawai.NamaPegawai}', no_ic='{pegawai.NoIc}', id_gred={pegawai.IdGred}, " +
                    $"id_bahagian={pegawai.IdBahagian}, id_cawangan={(pegawai.IdCawangan != null ? $"{pegawai.IdCawangan}" : "NULL")}, " +
                    $"id_unit={(pegawai.IdUnit != null ? $"{pegawai.IdUnit}" : "NULL")}, id_stesen={(pegawai.IdStesen != null ? $"{pegawai.IdStesen}" : "NULL")} " +
                    $"WHERE id= {pegawai.Id};";

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
