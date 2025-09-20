using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Repositories;
using Microsoft.Data.SqlClient;

namespace AllInOneProject.Services
{
    public interface IPurchaseService
    {
        Task<ServiceResponse<PurchaseDTO>> GetPurchaseMasterDataByIdAsync(int id);
        Task<ServiceResponse<int>> SavePurchaseDataAsync(PurchaseMasterRequest request);
        Task<ServiceResponse<int>> UpdatePurchaseDataAsync(PurchaseMasterRequest request);
        Task<ServiceResponse<List<PurchaseDTO>>> GetPurchaseDataListAsync();
        Task<ServiceResponse<bool>> DeletePurchaseDataAsync(int id);
    }

    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public PurchaseService(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }
        public async Task<ServiceResponse<PurchaseDTO>> GetPurchaseMasterDataByIdAsync(int id)
        {
            try
            {
                var result = await _purchaseRepository.GetPurchaseMasterDataByIdAsync(id);
                if (result == null)
                {
                    return new ServiceResponse<PurchaseDTO>
                    {
                        Success = false,
                        Message = "Purchase not found."
                    };
                }
                var purchaseDto = new PurchaseDTO
                {
                    Id = result.Id,
                    PurchaseDate = result.PurchaseDate,
                    PartyMasterId = result.PartyMasterId,
                    purchaseDetails = result.purchaseDetails.Select(d => new PurchaseDetailDTO
                    {
                        Id = d.Id,
                        ItemId = d.ItemId,
                        PurchaseMasterId = d.PurchaseMasterId,
                        Qty = d.Qty,
                        Rate = d.ItemMaster.Price,
                        Amount = d.Qty * d.ItemMaster.Price,
                        ItemMaster = new ItemDto
                        {
                            Id = d.ItemMaster.Id,
                            Name = d.ItemMaster.Name,
                            Price = d.ItemMaster.Price
                        }
                    }).ToList()
                };                
                return new ServiceResponse<PurchaseDTO>
                {
                    Success = true,
                    Message = "Purchase data retrieved successfully.",
                    Data = purchaseDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PurchaseDTO>
                {
                    Success = false,
                    Message = $"Error in retriving Purchase data: {ex.Message}"
                };
            }
        }
        public async Task<ServiceResponse<int>> SavePurchaseDataAsync(PurchaseMasterRequest request)
        {
            if (request == null || request.PurchaseDetailRequests == null || !request.PurchaseDetailRequests.Any())
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Invalid purchase data.",
                    Data = 0
                };
            }

            try
            {
                // Map DTO -> Entity
                var purchaseMaster = new PurchaseMaster
                {
                    PurchaseDate = request.PurchaseDate,
                    PartyMasterId = request.PartyMasterId,
                    purchaseDetails = request.PurchaseDetailRequests.Select(d => new PurchaseDetail
                    {
                        PurchaseMasterId = d.PurchaseMasterId,
                        ItemId = d.ItemId,
                        Qty = d.Qty
                    }).ToList()
                };

                // Call repository with Entity
                var result = await _purchaseRepository.SavePurchaseMasterAsync(purchaseMaster);

                return new ServiceResponse<int>
                {
                    Success = true,
                    Message = "Purchase data saved successfully",
                    Data = result
                };
            }
            catch (SqlException sqlEx)
            {
                // SQL / DB related errors
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Database error: {sqlEx.Message}",
                    Data = 0
                };
            }
            catch (InvalidOperationException invEx)
            {
                // Invalid operations (e.g. connection not open, invalid state)
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Operation error: {invEx.Message}",
                    Data = 0
                };
            }
            catch (ArgumentException argEx)
            {
                // Wrong arguments passed (already used in repository validation)
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Invalid argument: {argEx.Message}",
                    Data = 0
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = 0
                };
            }
        }
        public async Task<ServiceResponse<int>> UpdatePurchaseDataAsync(PurchaseMasterRequest request)
        {
            if (request == null || request.PurchaseDetailRequests == null || !request.PurchaseDetailRequests.Any())
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Invalid purchase data.",
                    Data = 0
                };
            }

            try
            {
                // Map DTO -> Entity
                var purchaseMaster = new PurchaseMaster
                {
                    Id = request.Id,
                    PurchaseDate = request.PurchaseDate,
                    PartyMasterId = request.PartyMasterId,
                    purchaseDetails = request.PurchaseDetailRequests.Select(d => new PurchaseDetail
                    {
                        PurchaseMasterId = d.PurchaseMasterId,
                        ItemId = d.ItemId,
                        Qty = d.Qty
                    }).ToList()
                };

                // Call repository with Entity
                var result = await _purchaseRepository.UpdatePurchaseMasterAsync(purchaseMaster);

                return new ServiceResponse<int>
                {
                    Success = true,
                    Message = "Purchase data updated successfully",
                    Data = result
                };
            }
            catch (SqlException sqlEx)
            {
                // SQL / DB related errors
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Database error: {sqlEx.Message}",
                    Data = 0
                };
            }
            catch (InvalidOperationException invEx)
            {
                // Invalid operations (e.g. connection not open, invalid state)
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Operation error: {invEx.Message}",
                    Data = 0
                };
            }
            catch (ArgumentException argEx)
            {
                // Wrong arguments passed (already used in repository validation)
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Invalid argument: {argEx.Message}",
                    Data = 0
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = 0
                };
            }
        }
        public async Task<ServiceResponse<List<PurchaseDTO>>> GetPurchaseDataListAsync()
        {
            try
            {
                var result = await _purchaseRepository.GetPurchaseDataListAsync();

                var purchaseListsDtos = new List<PurchaseDTO>();
                foreach (var purchase in result)
                {
                    purchaseListsDtos.Add(new PurchaseDTO
                    {
                        Id = purchase.Id,
                        PurchaseDate = purchase.PurchaseDate,
                        PartyMasterId = purchase.PartyMasterId,
                        PartyName = purchase.PartyMaster.Name,
                        purchaseDetails = purchase.purchaseDetails.Select(d => new PurchaseDetailDTO
                        {
                            Id = d.Id,
                            ItemId = d.ItemId,
                            PurchaseMasterId = d.PurchaseMasterId,
                            Qty = d.Qty,
                            Rate = d.ItemMaster.Price,
                            Amount = d.Qty * d.ItemMaster.Price,
                            ItemMaster = new ItemDto
                            {
                                Id = d.ItemMaster.Id,
                                Name = d.ItemMaster.Name,
                                Price = d.ItemMaster.Price
                            }
                        }).ToList()
                    });
                }

                return new ServiceResponse<List<PurchaseDTO>>
                {
                    Success = true,
                    Message = "Purchase data retrieved successfully.",
                    Data = purchaseListsDtos
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<PurchaseDTO>>
                {
                    Success = false,
                    Message = $"Error in retriving Purchase data: {ex.Message}"
                };
            }
        }
        public async Task<ServiceResponse<bool>> DeletePurchaseDataAsync(int id)
        {
            try
            {
                var result = await _purchaseRepository.DeletePurhcaseDataAsync(id);
                if (!result)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = $"Purchsse data with Id {id} not found."
                    };
                }

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Purchase data deleted successfully",
                    Data = result  // e.g., number of rows affected
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting Purchase data: {ex.Message}"
                };
            }
        }
    }
}
