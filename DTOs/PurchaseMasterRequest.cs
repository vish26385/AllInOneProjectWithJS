using AllInOneProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.DTOs
{
    public class PurchaseMasterRequest
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int PartyMasterId { get; set; }
        public List<PurchaseDetailRequest> PurchaseDetailRequests { get; set; } = new List<PurchaseDetailRequest>();
        public List<int> DeletedPurchaseDetailIds { get; set; }
    }
}
