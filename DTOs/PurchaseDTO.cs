using AllInOneProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.DTOs
{
    public class PurchaseDTO
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }        
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public List<PurchaseDetailDTO> purchaseDetails { get; set; } = new List<PurchaseDetailDTO>();        
    }
}
