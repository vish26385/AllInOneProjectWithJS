using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AllInOneProject.Services
{
    public class PartyService : IPartyService
    {
        private readonly IPartyRepository _partyRepository;
        public PartyService(IPartyRepository partyRepository)
        {
            _partyRepository = partyRepository;
        }

        public async Task<ServiceResponse<List<PartyMaster>>> GetAllPartiesAsync()
        {
            var parties = await _partyRepository.GetAllPartiesAsync();

            if (parties == null || parties.Count == 0)
            {
                return new ServiceResponse<List<PartyMaster>>
                {
                    Success = false,
                    Message = "No parties found",
                    Data = new List<PartyMaster>()
                };
            }

            return new ServiceResponse<List<PartyMaster>>
            {
                Success = true,
                Message = "Parties retrieved successfully",
                Data = parties
            };
        }

        public async Task<ServiceResponse<int>> SavePartyAsync(PartyMasterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PartyMaster.Name))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Party name is required."
                };
            }
            var result = await _partyRepository.SavePartyAsync(request);
            return new ServiceResponse<int>
            {
                Success = true,
                Message = "Party saved successfully",
                Data = result  // e.g., number of rows affected or new PartyId
            };
        }

        public async Task<PartyMasterRequest> GetEditPartyModelAsync(int id)
        {
            var party = await _partyRepository.GetPartyByIdAsync(id);
            var parties = await _partyRepository.GetAllPartiesAsync();

            return new PartyMasterRequest
            {
                PartyMaster = party,       // Fill top inputs
                partyMasters = parties     // Fill table list
            };
        }

        public async Task<ServiceResponse<int>> UpdatePartyAsync(PartyMasterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PartyMaster.Name))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Party name is required."
                };
            }
            var result = await _partyRepository.UpdatePartyAsync(request);
            return new ServiceResponse<int>
            {
                Success = true,
                Message = "Party updated successfully",
                Data = result  // e.g., number of rows affected
            };
        }

        public async Task<ServiceResponse<int>> DeletePartyAsync(int id)
        {
            var result = await _partyRepository.DeletePartyAsync(id);
            if (result == 0)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "No parties found",
                    Data = 0
                };
            }
            return new ServiceResponse<int>
            {
                Success = true,
                Message = "Party deleted successfully",
                Data = result  // e.g., number of rows affected
            };
        }
    }
}
