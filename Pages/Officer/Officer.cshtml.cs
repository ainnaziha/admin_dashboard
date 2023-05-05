using Microsoft.AspNetCore.Mvc.RazorPages;
using spl.Model;
using System.Data.SqlClient;
using System.Diagnostics;

namespace spl.Pages.Officer
{
    public class OfficerModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string? Layout { get; private set; }
        public List<Pegawai> listPegawai = new();
        public OfficerModel(IConfiguration config)
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
        }

        public void FetchOfficer()
        {
            Debug.WriteLine("Officer FetchOfficer: Fetch grade list");

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
                    "LEFT JOIN stesen s ON p.id_stesen = s.id";

                using SqlCommand command = new(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Pegawai pegawai = new()
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

                        listPegawai.Add(pegawai);
                    }

                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Officer FetchOfficer Error: {ex.Message}");
            }
        }
    }
}