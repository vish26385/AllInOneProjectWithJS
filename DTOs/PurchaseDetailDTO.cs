using AllInOneProject.DTOs;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.DTOs
{
    public class PurchaseDetailDTO
    {
        public int Id { get; set; }       
        public int PurchaseMasterId { get; set; }       
        public int ItemId { get; set; }
        public int Qty { get; set; }       
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public ItemDto ItemMaster { get; set; }
    }
}
