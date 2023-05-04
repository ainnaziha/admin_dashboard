namespace spl.Model
{
    public class Bahagian
    {
        public int Id { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
