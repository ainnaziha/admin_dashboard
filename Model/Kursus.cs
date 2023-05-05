namespace spl.Model
{
    public class Kursus
    {
        public int? Id { get; set; }
        public String Tajuk { get; set; }
        public String TarikhMula { get; set; }
        public String TarikhAkhir { get; set; }
        public String Lokasi { get; set; }
        public bool? IsDeleted { get; set; }
        public int? IdKategori { get; set; }
        public KategoriKursus? KategoriKursus { get; set; }
    }
}
