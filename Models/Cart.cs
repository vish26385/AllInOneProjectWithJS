using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.Models
{
    public class Cart
    {
        public int Id { get; set; }        
        public int ItemId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }   // changed from int → string
        public ApplicationUser User { get; set; }   // navigation property
        public int Quantity { get; set; }   // ✅ Add this
        public Item item { get; set; }
    }
}
