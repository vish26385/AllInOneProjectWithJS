using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }   // changed from int → string
        public ApplicationUser User { get; set; }   // navigation property
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
