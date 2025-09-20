using AllInOneProject.Models;

namespace AllInOneProject.DTOs
{
    public class PartyMasterRequest
    {
        public int Id { get; set; }     // used for update
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
