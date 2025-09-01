using AllInOneProject.DTOs;
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
            try
            {
                var result = await _repository.SaveSalesDataAsync(request);
                return new ServiceResponse<int>
                {
                    Success = true,
                    Message = "Sales data saved successfully",
                    Data = result  // e.g., number of rows affected or new SaleMasterId
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = 0 // e.g., number of rows affected or new SaleMasterId
                };
            }            
        }
        public async Task<ServiceResponse<int>> UpdateSalesDataAsync(SalesMasterRequest request)
        {
            try
            {
                var result = await _repository.UpdateSalesDataAsync(request);
                return new ServiceResponse<int>
                {
                    Success = true,
                    Message = "Sales data updated successfully",
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
    }
}
