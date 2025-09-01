using AllInOneProject.Models;

namespace AllInOneProject.DTOs
{
    public class PartyMasterRequest
    {
        public PartyMaster PartyMaster { get; set; }
        public List<PartyMaster> partyMasters { get; set; }
    }
}
