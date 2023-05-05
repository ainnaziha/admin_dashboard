namespace spl.Model
{
    public class KursusPegawai
    {
        public int? Id { get; set; }
        public String TarikhMula { get; set; }
        public String TarikhAkhir { get; set; }
        public int? IdPegawai { get; set; }
        public int? IdKursus { get; set; }
        public double JumlahHari { get; set; }
        public int Bulan { get; set; }
        public Pegawai? Pegawai { get; set; }
        public Kursus? Kursus { get; set; }
    }
}