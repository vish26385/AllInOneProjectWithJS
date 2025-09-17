namespace AllInOneProject.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }   // ✅ Add this
        public Item item { get; set; }
    }
}
