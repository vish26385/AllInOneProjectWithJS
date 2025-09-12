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

        public async Task<ServiceResponse<PartyMasterDTO>> GetPartyByIdAsync(int id)
        {
            var party = await _partyRepository.GetPartyByIdAsync(id);
            if (party == null)
            {
                return new ServiceResponse<PartyMasterDTO>
                {
                    Success = false,
                    Message = "No party found",
                    Data = new PartyMasterDTO()
                };
            }

            var dtoList = new PartyMasterDTO
            {
                Id = party.Id,
                Name = party.Name
            };

            return new ServiceResponse<PartyMasterDTO>
            {
                Success = true,
                Message = "Party retrieved successfully",
                Data = dtoList
            };
        }

        public async Task<ServiceResponse<List<PartyMasterDTO>>> GetAllPartiesAsync()
        {
            var parties = await _partyRepository.GetAllPartiesAsync();

            if (parties == null || parties.Count == 0)
            {
                return new ServiceResponse<List<PartyMasterDTO>>
                {
                    Success = false,
                    Message = "No parties found",
                    Data = new List<PartyMasterDTO>()
                };
            }

            // Map Entity → DTO
            var dtoList = parties.Select(p => new PartyMasterDTO
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();

            return new ServiceResponse<List<PartyMasterDTO>>
            {
                Success = true,
                Message = "Parties retrieved successfully",
                Data = dtoList
            };
        }

        public async Task<ServiceResponse<int>> SavePartyAsync(PartyMasterRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Party name is required."
                };
            }

            // Map DTO → Model(Entity)
            var party = new PartyMaster
            {
                Name = request.Name
            };

            var result = await _partyRepository.SavePartyAsync(party);

            return new ServiceResponse<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Party saved successfully" : "Failed to save party",
                Data = result
            };
        }

        public async Task<ServiceResponse<int>> UpdatePartyAsync(PartyMasterRequest request)
        {
            if (request == null)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Invalid request."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Party name is required."
                };
            }

            // Map DTO → Entity
            var party = new PartyMaster
            {
                Id = request.Id,
                Name = request.Name
            };

            var result = await _partyRepository.UpdatePartyAsync(party);

            return new ServiceResponse<int>
            {
                Success = result > 0,
                Message = result > 0 ? "Party updated successfully" : "No record updated",
                Data = result
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
