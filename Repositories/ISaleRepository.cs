using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Repositories
{
    public interface ISaleRepository
    {
        Task<SalesMaster> GetSaleMasterDataByIdAsync(int id);
        Task<SalesDetail> GetSaleDetailDataByIdAsync(int id);
        Task<int> SaveSaleMasterAsync(SalesMaster saleMaster);
        Task<int> UpdateSaleMasterAsync(SalesMaster saleMaster, List<int>? deletedDetailIds);
        Task<int> DeleteSalesDataAsync(int id);
        Task<List<SaleDto>> GetSaleDataListAsync();
    }
}
