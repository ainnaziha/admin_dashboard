namespace spl.Model
{
    public class Unit
    {
        public int Id { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}
