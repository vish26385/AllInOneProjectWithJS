namespace AllInOneProject.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public Item item { get; set; }
    }
}
