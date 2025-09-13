using AllInOneProject.DTOs;

namespace AllInOneProject.Services
{
    public interface ISaleService
    {
        Task<ServiceResponse<int>> SaveSalesDataAsync(SalesMasterRequest request);
        Task<ServiceResponse<int>> UpdateSalesDataAsync(SalesMasterRequest request);
        Task<ServiceResponse<bool>> DeleteSalesDataAsync(int id);
        Task<List<SaleDto>> GetSaleDataListAsync();
    }
}
