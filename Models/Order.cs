using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
