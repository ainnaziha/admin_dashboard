namespace spl.Model
{
    public class Bahagian
    {
        public int? Id { get; set; }
        public String NamaBahagian { get; set; }

        public ICollection<Cawangan>? Cawangans { get; set; }
        public ICollection<Unit>? Units { get; set; }

    }
}
