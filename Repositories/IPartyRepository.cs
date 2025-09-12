using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Repositories
{
    public interface IPartyRepository
    {
        Task<PartyMaster?> GetPartyByIdAsync(int id);
        Task<List<PartyMaster>> GetAllPartiesAsync();
        Task<int> SavePartyAsync(PartyMaster party);
        Task<int> UpdatePartyAsync(PartyMaster party);
        Task<int> DeletePartyAsync(int Id);
    }
}
