namespace AllInOneProject.DTOs
{
    public class SalesDetailRequest
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public int SaleMasterId { get; set; } = 0;
    }
}
