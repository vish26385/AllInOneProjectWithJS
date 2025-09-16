using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Services
{
    public interface IPartyService
    {
        Task<ServiceResponse<PartyMasterDTO>> GetPartyByIdAsync(int id);

        Task<ServiceResponse<List<PartyMasterDTO>>> GetAllPartiesAsync();

        Task<ServiceResponse<PartyMasterDTO>> SavePartyAsync(PartyMasterRequest request);        

        Task<ServiceResponse<bool>> UpdatePartyAsync(PartyMasterRequest request);

        Task<ServiceResponse<bool>> DeletePartyAsync(int id);
    }
}
