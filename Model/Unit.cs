namespace spl.Model
{
    public class Unit
    {
        public int? Id { get; set; }
        public String NamaUnit { get; set; }
        public int? IdBahagian { get; set; }

        public Bahagian? Bahagian { get; set; }
    }
}
