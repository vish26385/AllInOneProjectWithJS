using AllInOneProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }        
        public int OrderId { get; set; }       
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public ItemDto Item { get; set; }
    }
}
