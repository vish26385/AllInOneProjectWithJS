namespace AllInOneProject.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public int DueDays { get; set; }
        public DateTime DueDate { get; set; }
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public int Qty { get; set; }
        public decimal Amount { get; set; }
        public List<SaleDetailDto> salesDetails { get; set; }

    }
}
