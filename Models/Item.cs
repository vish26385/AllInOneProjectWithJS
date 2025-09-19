namespace AllInOneProject.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal CurrentStock { get; set; } = decimal.Zero;
    }
}
