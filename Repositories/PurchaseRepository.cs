using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics.Metrics;

namespace AllInOneProject.Repositories
{
    public interface IPurchaseRepository
    {
        Task<PurchaseMaster> GetPurchaseMasterDataByIdAsync(int id);
        Task<int> SavePurchaseMasterAsync(PurchaseMaster purchaseMaster);
        Task<int> UpdatePurchaseMasterAsync(PurchaseMaster purchaseMaster);
        Task<List<PurchaseMaster>> GetPurchaseDataListAsync();
        Task<bool> DeletePurhcaseDataAsync(int id);
    }

    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public PurchaseRepository(IConfiguration configuration, ApplicationDbContext context)
        {
            _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
       ?? configuration.GetConnectionString("ConnectionString");
            _context = context;
        }
        public async Task<PurchaseMaster> GetPurchaseMasterDataByIdAsync(int id)
        {
            return await _context.PurchaseMaster
                .Include(s => s.PartyMaster)
                .Include(d => d.purchaseDetails)
                .ThenInclude(d => d.ItemMaster)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<int> SavePurchaseMasterAsync(PurchaseMaster purchaseMaster)
        {
            int PurchaseMasterId = 0;

            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var trn = await con.BeginTransactionAsync())
                {
                    try
                    {
                        // Insert into PurchaseMaster
                        using (var cmd = new SqlCommand("sp_insertPurchaseMaster", con, (SqlTransaction)trn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@PurchaseDate", purchaseMaster.PurchaseDate);
                            cmd.Parameters.AddWithValue("@PartyMasterId", purchaseMaster.PartyMasterId);
                            cmd.Parameters.Add("@PurchaseMasterId", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;

                            await cmd.ExecuteNonQueryAsync();
                            PurchaseMasterId = (int)cmd.Parameters["@PurchaseMasterId"].Value;
                        }

                        // Insert PurchaseDetails
                        foreach (var item in purchaseMaster.purchaseDetails)
                        {
                            using var cmdDetail = new SqlCommand("sp_insertPurchaseDetail", con, (SqlTransaction)trn);
                            cmdDetail.CommandType = CommandType.StoredProcedure;
                            cmdDetail.Parameters.AddWithValue("@PurchaseMasterId", PurchaseMasterId);
                            cmdDetail.Parameters.AddWithValue("@ItemId", item.ItemId);
                            cmdDetail.Parameters.AddWithValue("@Qty", item.Qty);

                            await cmdDetail.ExecuteNonQueryAsync();
                        }

                        await trn.CommitAsync();
                    }
                    catch
                    {
                        await trn.RollbackAsync();
                        throw; // rethrow to service
                    }
                }
            }

            return PurchaseMasterId;
        }
        public async Task<int> UpdatePurchaseMasterAsync(PurchaseMaster purchaseMaster)
        {
            var existing = await GetPurchaseMasterDataByIdAsync(purchaseMaster.Id);

            if (existing == null)
                return 0;

            existing.PurchaseDate = purchaseMaster.PurchaseDate;
            existing.PartyMasterId = purchaseMaster.PartyMasterId;

            var itemMaster = new Item();
            _context.PurchaseDetails.RemoveRange(existing.purchaseDetails);
            foreach (var detail in existing.purchaseDetails)
            {
                if (detail.ItemId > 0)
                {
                    itemMaster = _context.Items?.Find(detail.ItemId);
                    itemMaster.CurrentStock = itemMaster.CurrentStock - (decimal)detail.Qty;
                    _context.Entry(itemMaster).State = EntityState.Modified;
                }
            }
            await _context.SaveChangesAsync();

            if (purchaseMaster.purchaseDetails != null)
            {
                foreach (var detail in purchaseMaster.purchaseDetails)
                {
                    if (detail.ItemId > 0)
                    {
                        _context.PurchaseDetails.Add(new PurchaseDetail { PurchaseMasterId = existing.Id, Qty = detail.Qty, ItemId = detail.ItemId });
                        itemMaster = _context.Items?.Find(detail.ItemId);
                        itemMaster.CurrentStock = itemMaster.CurrentStock + (decimal)detail.Qty;
                        _context.Entry(itemMaster).State = EntityState.Modified;
                    }
                }
            }
            await _context.SaveChangesAsync();

            return purchaseMaster.Id;
        }
        public async Task<List<PurchaseMaster>> GetPurchaseDataListAsync()
        {
            var purchaseLists = await _context.PurchaseMaster
                               .Include(s => s.PartyMaster)
                               .Include(s => s.purchaseDetails)
                               .ThenInclude(d => d.ItemMaster)
                               .OrderByDescending(s => s.Id)
                               .ToListAsync();

            return purchaseLists;
        }
        public async Task<bool> DeletePurhcaseDataAsync(int id)
        {
            var existing = await GetPurchaseMasterDataByIdAsync(id);

            if (existing == null)
                return false;

            _context.PurchaseDetails.RemoveRange(existing.purchaseDetails);
            var itemMaster = new Item();
            foreach (var detail in existing.purchaseDetails)
            {
                if (detail.ItemId > 0)
                {
                    itemMaster = _context.Items?.Find(detail.ItemId);
                    itemMaster.CurrentStock = itemMaster.CurrentStock - (decimal)detail.Qty;
                    _context.Entry(itemMaster).State = EntityState.Modified;
                }
            }
            _context.PurchaseMaster.Remove(existing);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
