namespace AllInOneProject.DTOs
{
    public class SalesMasterRequest
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public int DueDays { get; set; }
        public DateTime DueDate { get; set; }
        public int PartyMasterId { get; set; }
        public List<SalesDetailRequest> SalesDetailRequests { get; set; }
        public List<int> DeletedSaleDetailIds { get; set; }
    }
}
