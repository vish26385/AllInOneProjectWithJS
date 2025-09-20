using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.Models
{
    public class PurchaseDetail
    {
        public int Id { get; set; }
        public int PurchaseMasterId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
        public Item ItemMaster { get; set; }
        public PurchaseMaster PurchaseMaster { get; set; }

        [NotMapped]
        public decimal Rate { get; set; }

        [NotMapped]
        public decimal Amount { get; set; }
    }
}
