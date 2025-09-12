using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Services
{
    public interface IPartyService
    {
        Task<ServiceResponse<PartyMasterDTO>> GetPartyByIdAsync(int id);

        Task<ServiceResponse<List<PartyMasterDTO>>> GetAllPartiesAsync();

        Task<ServiceResponse<int>> SavePartyAsync(PartyMasterRequest request);        

        Task<ServiceResponse<int>> UpdatePartyAsync(PartyMasterRequest request);

        Task<ServiceResponse<int>> DeletePartyAsync(int id);
    }
}
