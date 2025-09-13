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

        public async Task<ServiceResponse<bool>> UpdatePartyAsync(PartyMasterRequest request)
        {
            if (request == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Invalid request."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Party name is required."
                };
            }

            try
            {
                // Map DTO → Entity
                var party = new PartyMaster
                {
                    Id = request.Id,
                    Name = request.Name
                };

                var result = await _partyRepository.UpdatePartyAsync(party);

                if (!result)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = $"Party with Id={request.Id} not found or no changes applied."
                    };
                }

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Party updated successfully",
                    Data = result
                };
            }
            catch (SqlException sqlEx)
            {
                // SQL / DB related errors
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Database error: {sqlEx.Message}"
                };
            }
            catch (InvalidOperationException invEx)
            {
                // Invalid operations (e.g. connection not open, invalid state)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Operation error: {invEx.Message}"
                };
            }
            catch (ArgumentException argEx)
            {
                // Wrong arguments passed (already used in repository validation)
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Invalid argument: {argEx.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error while updating party: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeletePartyAsync(int id)
        {
            try
            {
                var result = await _partyRepository.DeletePartyAsync(id);
                if (!result)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = $"Party data with Id {id} not found."
                    };
                }
                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Party deleted successfully",
                    Data = result  // e.g., number of rows affected
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting party data: {ex.Message}"
                };
            }
        }
    }
}
