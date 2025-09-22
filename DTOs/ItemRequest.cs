using System.ComponentModel.DataAnnotations;

namespace AllInOneProject.DTOs
{
    public class ItemRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Item name is required")]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal? Price { get; set; }
        public decimal? CurrentStock { get; set; }
    }
}
