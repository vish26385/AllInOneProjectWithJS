using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Repositories;

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
                    PartyId = request.PartyId,
                    salesDetails = request.SalesDetailRequests.Select(d => new SalesDetail
                    {
                        itemId = d.ItemId,
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
                saleMaster.PartyId = request.PartyId;
                saleMaster.salesDetails = request.SalesDetailRequests.Select(d => new SalesDetail
                {
                    Id = d.Id,
                    SalesMasterId = d.SaleMasterId,
                    itemId = d.ItemId,
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
        public async Task<ServiceResponse<int>> DeleteSalesDataAsync(int id)
        {
            try
            {
                var result = await _repository.DeleteSalesDataAsync(id);
                return new ServiceResponse<int>
                {
                    Success = true,
                    Message = "Sales data deleted successfully",
                    Data = result  // e.g., number of rows affected
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = 0  // e.g., number of rows affected
                };
            }
        }
        public async Task<List<SaleDto>> GetSaleDataListAsync()
        {
            return await _repository.GetSaleDataListAsync();
        }
    }
}
