using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.Models
{
    public class PurchaseMaster
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        [ForeignKey("PartyMaster")]
        public int PartyId { get; set; }
        public PartyMaster PartyMaster { get; set; }
        public List<PurchaseDetail> purchaseDetails { get; set; } = new List<PurchaseDetail>();
    }
}
