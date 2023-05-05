namespace spl.Model
{
    public class Pegawai
    {
        public int? Id { get; set; }
        public string NamaPegawai { get; set; }
        public string NoIc { get; set; }
        public int? IdGred { get; set; }
        public int? IdBahagian { get; set; }
        public int? IdCawangan { get; set; }
        public int? IdUnit { get; set; }
        public int? IdStesen { get; set; }

        public Gred? Gred { get; set; }
        public Bahagian? Bahagian { get; set; }
        public Cawangan? Cawangan { get; set; }
        public Unit? Unit { get; set; }
        public Stesen? Stesen { get; set; }
    }
}
