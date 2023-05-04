namespace spl.Model
{
    public class User
    {
        public int? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? UserType { get; set; }
        public int? BahagianId { get; set; }
        public int? CawanganId { get; set; }
        public int? UnitId { get; set; }
        public int? StesenId { get; set; }

        public Bahagian? Bahagian { get; set; }
        public Cawangan? Cawangan { get; set; }
        public Unit? Unit { get; set; }
        public Stesen? Stesen { get; set; }
    }
}
