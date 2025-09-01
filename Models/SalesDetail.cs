using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.Models
{
    public class SalesDetail
    {
        public int Id { get; set; }
        [ForeignKey("SalesMaster")]
        public int SalesMasterId { get; set; }
        [ForeignKey("Item")]
        public int itemId { get; set; }
        public int Qty { get; set; }
        public SalesMaster SalesMaster { get; set; }
        public Item ItemMaster { get; set; }
    }
}
