using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using spl.Model;
using System.Data.SqlClient;
using System.Diagnostics;

namespace spl.Pages.Officer
{
    [IgnoreAntiforgeryToken]
    public class OfficerCourseModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<KursusPegawai> list = new();
        public Pegawai? pegawai { get; private set; }
        public string url = "";

        public OfficerCourseModel(IConfiguration config)
        {
            _configuration = config;
        }

        public void OnGet()
        {
            url = $"/officer/addofficercourse?id={Request.Query["id"]}";
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
            FetchCourse();
        }

        public void FetchOfficer()
        {
            Debug.WriteLine("OfficerCourse FetchOfficer: Fetch officer");

            String id = Request.Query["id"];

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();
                String sql = "SELECT p.id, p.nama_pegawai, p.no_ic, p.id_gred, g.abjad, g.nombor, g.gelaran_pangkat, " +
                    "p.id_bahagian, b.nama_bahagian, p.id_cawangan, c.nama_cawangan, p.id_unit, u.nama_unit, p.id_stesen, s.nama_stesen " +
                    "FROM pegawai p " +
                    "LEFT JOIN gred g ON p.id_gred = g.id " +
                    "LEFT JOIN bahagian b ON p.id_bahagian = b.id " +
                    "LEFT JOIN cawangan c ON p.id_cawangan = c.id " +
                    "LEFT JOIN unit u ON p.id_unit = u.id " +
                    "LEFT JOIN stesen s ON p.id_stesen = s.id " +
                    $"WHERE p.id = {id};";

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
                            Gred = reader["id_gred"] == DBNull.Value ? null : new Gred()
                            {
                                Id = reader["id_gred"] == DBNull.Value ? null : Convert.ToInt32(reader["id_gred"]),
                                Abjad = Convert.ToString(reader["abjad"]) ?? "",
                                Nombor = Convert.ToString(reader["nombor"]) ?? "",
                                GelaranPangkat = reader["gelaran_pangkat"] == DBNull.Value ? null : Convert.ToString(reader["gelaran_pangkat"])
                            },
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
                    }

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OfficerCourse FetchOfficer Error: {ex.Message}");
            }
        }

        public void FetchCourse()
        {
            Debug.WriteLine("OfficerCourse FetchCourse: Fetch course list");

            String id = Request.Query["id"];

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = "SELECT kp.id, kp.tarikh_mula, kp.tarikh_akhir, kp.jumlah_hari, " +
                    "kp.id_kursus, k.tajuk, k.tarikh_mula AS t_mula, k.tarikh_akhir AS t_akhir, k.lokasi, " +
                    "kp.id_pegawai " +
                    "FROM kursus_pegawai kp " +
                    "JOIN kursus k ON kp.id_kursus = k.id " +
                    $"WHERE (kp.is_deleted IS NULL OR kp.is_deleted <> 1) AND kp.id_pegawai = {id};";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime tMula = DateTime.Parse(Convert.ToString(reader["tarikh_mula"]) ?? "");
                        DateTime tAkhir = DateTime.Parse(Convert.ToString(reader["tarikh_akhir"]) ?? "");

                        DateTime tM = DateTime.Parse(Convert.ToString(reader["t_mula"]) ?? "");
                        DateTime tA = DateTime.Parse(Convert.ToString(reader["t_akhir"]) ?? "");

                        KursusPegawai kp = new()
                        {
                            Id = reader["id"] == DBNull.Value ? null : Convert.ToInt32(reader["id"]),
                            TarikhMula = tMula.ToString("dd/MM/yyyy"),
                            TarikhAkhir = tAkhir.ToString("dd/MM/yyyy"),
                            JumlahHari = reader["jumlah_hari"] == DBNull.Value ? 0 : Convert.ToDouble(reader["jumlah_hari"]),
                            IdPegawai = reader["id_pegawai"] == DBNull.Value ? null : Convert.ToInt32(reader["id_pegawai"]),
                            Kursus = reader["id_kursus"] == DBNull.Value ? null : new Kursus()
                            {
                                Id = reader["id_kursus"] == DBNull.Value ? null : Convert.ToInt32(reader["id_kursus"]),
                                Tajuk = Convert.ToString(reader["tajuk"]) ?? "",
                                TarikhMula = tM.ToString("dd/MM/yyyy"),
                                TarikhAkhir = tA.ToString("dd/MM/yyyy"),
                                Lokasi = Convert.ToString(reader["lokasi"]) ?? "",
                            }
                        };

                        list.Add(kp);
                    }

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"OfficerCourse FetchCourse Error: {ex.Message}");
            }
        }

        public JsonResult OnPostDelete(int id)
        {
            Debug.WriteLine($"OfficerCourse OnPostDelete: Delete item {id}");

            try
            {
                String connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection connection = new(connectionString);
                connection.Open();

                String sql = $"UPDATE kursus_pegawai SET is_deleted = 1 WHERE id = {id};";

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