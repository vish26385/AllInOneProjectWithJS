using AllInOneProject.Models;

namespace AllInOneProject.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string UserId { get; set; }
        public int Quantity { get; set; }   // ✅ Add this
        public ItemDto item { get; set; }
    }
}
