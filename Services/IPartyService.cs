using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Services
{
    public interface IPartyService
    {
        Task<ServiceResponse<List<PartyMaster>>> GetAllPartiesAsync();

        Task<ServiceResponse<int>> SavePartyAsync(PartyMasterRequest request);

        Task<PartyMasterRequest> GetEditPartyModelAsync(int id);

        Task<ServiceResponse<int>> UpdatePartyAsync(PartyMasterRequest request);

        Task<ServiceResponse<int>> DeletePartyAsync(int id);
    }
}
