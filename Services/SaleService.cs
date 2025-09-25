using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Repositories;
using Microsoft.Data.SqlClient;

namespace AllInOneProject.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _repository;       
        public SaleService(ISaleRepository repository)
        {
            _repository = repository;
        }
        public async Task<ServiceResponse<int>> SaveSalesDataAsync(SalesMasterRequest request)
        {
            if (request == null || request.SalesDetailRequests == null || !request.SalesDetailRequests.Any())
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Invalid sale data.",
                    Data = 0
                };
            }

            try
            {
                // Map DTO -> Entity
                var saleMaster = new SalesMaster
                {
                    SalesDate = request.SaleDate,
                    DueDays = request.DueDays,
                    DueDate = request.DueDate,
                    PartyMasterId = request.PartyMasterId,
                    salesDetails = request.SalesDetailRequests.Select(d => new SalesDetail
                    {
                        ItemId = d.ItemId,
                        Qty = d.Qty
                    }).ToList()
                };

                // Call repository with Entity
                var result = await _repository.SaveSaleMasterAsync(saleMaster);

                return new ServiceResponse<int>
                {
                    Success = true,
                    Message = "Sales data saved successfully",
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
        public async Task<ServiceResponse<int>> UpdateSalesDataAsync(SalesMasterRequest request)
        {
            if (request == null || request.SalesDetailRequests == null || !request.SalesDetailRequests.Any())
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Invalid sale data.",
                    Data = 0
                };
            }

            try
            {
                var saleMaster = await _repository.GetSaleMasterDataByIdAsync(request.Id);
                if (saleMaster == null)
                {
                    return new ServiceResponse<int>
                    {
                        Success = false,
                        Message = $"SaleMaster with Id={request.Id} not found.",
                        Data = 0
                    };
                }

                // Map request -> entity
                saleMaster.SalesDate = request.SaleDate;
                saleMaster.DueDays = request.DueDays;
                saleMaster.DueDate = request.DueDate;
                saleMaster.PartyMasterId = request.PartyMasterId;
                saleMaster.salesDetails = request.SalesDetailRequests.Select(d => new SalesDetail
                {
                    Id = d.Id,
                    SalesMasterId = d.SaleMasterId,
                    ItemId = d.ItemId,
                    Qty = d.Qty
                }).ToList();
                
                // Call repository
                var result = await _repository.UpdateSaleMasterAsync(saleMaster, request.DeletedSaleDetailIds);

                return new ServiceResponse<int>
                {
                    Success = true,
                    Message = "Sales data updated successfully",
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
                    Message = $"Error while updating sales data: {ex.Message}",
                    Data = 0
                };
            }
        }
        public async Task<ServiceResponse<bool>> DeleteSalesDataAsync(int id)
        {
            try
            {
                var result = await _repository.DeleteSalesDataAsync(id);
                if (!result)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = $"Sales data with Id {id} not found."
                    };
                }

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Sales data deleted successfully",
                    Data = result  // e.g., number of rows affected
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting sales data: {ex.Message}"
                };
            }
        }
        public async Task<List<SaleDto>> GetSaleDataListAsync()
        {
            return await _repository.GetSaleDataListAsync();
        }
    }
}
