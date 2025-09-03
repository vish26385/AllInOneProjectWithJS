using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Repositories
{
    public interface ISaleRepository
    {
        Task<int> SaveSalesDataAsync(SalesMasterRequest request);
        Task<int> UpdateSalesDataAsync(SalesMasterRequest request);
        Task<int> DeleteSalesDataAsync(int id);
        Task<List<SaleDto>> GetSaleDataListAsync();
    }
}
