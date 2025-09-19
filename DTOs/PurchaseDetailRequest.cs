using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.DTOs
{
    public class PurchaseDetailRequest
    {
        public int Id { get; set; }       
        public int PurchaseMasterId { get; set; }
        public int ItemId { get; set; }
        public int Qty { get; set; }
    }
}
