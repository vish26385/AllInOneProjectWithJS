using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Repositories
{
    public interface IPartyRepository
    {
        Task<PartyMaster?> GetPartyByIdAsync(int id);
        Task<List<PartyMaster>> GetAllPartiesAsync();
        Task<int> SavePartyAsync(PartyMasterRequest request);
        Task<int> UpdatePartyAsync(PartyMasterRequest request);
        Task<int> DeletePartyAsync(int Id);
    }
}
