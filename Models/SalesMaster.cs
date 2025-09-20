using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.Models
{
    public class SalesMaster
    {
        public int Id { get; set; }
        public DateTime SalesDate { get; set; }
        public int DueDays { get; set; }
        public DateTime DueDate { get; set; }
        public int PartyMasterId { get; set; }
        public List<SalesDetail> salesDetails { get; set; } = new List<SalesDetail>();
        public PartyMaster PartyMaster { get; set; }
    }
}
