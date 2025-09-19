using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.ViewModels
{
    public class PurchaseViewModel
    {
        public PurchaseDTO purchaseMaster { get; set; } = new PurchaseDTO();
        public List<PurchaseDTO> purchaseLists { get; set; } = new List<PurchaseDTO>();
        public List<ItemDto> itemMasters { get; set; } = new List<ItemDto>();
        public List<PartyMasterDTO> partyMasters { get; set; } = new List<PartyMasterDTO>();
    }
}
