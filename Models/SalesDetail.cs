using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.Models
{
    public class SalesDetail
    {
        public int Id { get; set; }
        public int SalesMasterId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public SalesMaster SalesMaster { get; set; }
        public Item ItemMaster { get; set; }
    }
}
