using System.ComponentModel.DataAnnotations;

namespace AllInOneProject.ViewModels
{
    public class PartyMasterViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Party name is required")]
        [StringLength(100, ErrorMessage = "Party name cannot be longer than 100 characters")]
        public string Name { get; set; }

        public string Type { get; set; }

        // Optional: for listing multiple parties in a view (like index page)
        public List<PartyMasterViewModel>? partyMasters { get; set; }
    }
}
