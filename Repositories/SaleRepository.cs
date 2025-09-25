using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Services;
using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics.Metrics;

namespace AllInOneProject.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        private readonly IItemService _itemService;

        public SaleRepository(ApplicationDbContext context, string connectionString, IItemService itemService)
        {
            _connectionString = connectionString;
            _context = context;
            _itemService = itemService;
        }

        public async Task<SalesMaster> GetSaleMasterDataByIdAsync(int id)
        {
            return await _context.SalesMas.FindAsync(id);
        }

        public async Task<SalesDetail> GetSaleDetailDataByIdAsync(int id)
        {
            return await _context.SalesDet.FirstOrDefaultAsync(x => x.SalesMasterId == id);
        }

        public async Task<int> SaveSaleMasterAsync(SalesMaster saleMaster)
        {
            if (saleMaster == null || saleMaster.salesDetails == null || !saleMaster.salesDetails.Any())
                throw new ArgumentException("Invalid sale data.");

            int saleMasterId = 0;

            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var trn = await con.BeginTransactionAsync())
                {
                    try
                    {
                        // Insert into SaleMaster
                        using (var cmd = new SqlCommand("sp_InsertSaleMaster", con, (SqlTransaction)trn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SaleDate", saleMaster.SalesDate);
                            cmd.Parameters.AddWithValue("@DueDays", saleMaster.DueDays);
                            cmd.Parameters.AddWithValue("@DueDate", saleMaster.DueDate);
                            cmd.Parameters.AddWithValue("@PartyMasterId", saleMaster.PartyMasterId);
                            cmd.Parameters.Add("@SaleMasterId", SqlDbType.Int).Direction = ParameterDirection.Output;

                            await cmd.ExecuteNonQueryAsync();
                            saleMasterId = (int)cmd.Parameters["@SaleMasterId"].Value;
                        }

                        // Insert SaleDetails
                        foreach (var item in saleMaster.salesDetails)
                        {
                            using var cmdDetail = new SqlCommand("sp_InsertSaleDetail", con, (SqlTransaction)trn);
                            cmdDetail.CommandType = CommandType.StoredProcedure;
                            cmdDetail.Parameters.AddWithValue("@SaleMasterId", saleMasterId);
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

            return saleMasterId;
        }

        public async Task<int> UpdateSaleMasterAsync(SalesMaster saleMaster, List<int>? deletedDetailIds)
        {
            if (saleMaster == null)
                throw new ArgumentException("Invalid sale data.");

            var itemMaster = new Item();
            var detailsToDelete = _context.SalesDet
            .Where(d => deletedDetailIds.Contains(d.Id))
            .ToList();

            _context.SalesDet.RemoveRange(detailsToDelete);

            var itemIds = detailsToDelete
                .Where(d => d.ItemId > 0)
                .Select(d => d.ItemId)
                .Distinct()
                .ToList();

            var items = _context.Items
                .Where(i => itemIds.Contains(i.Id))
                .ToDictionary(i => i.Id);

            foreach (var detail in detailsToDelete)
            {
                if (detail.ItemId > 0 && items.TryGetValue(detail.ItemId, out var itemMasters))
                {
                    itemMasters.CurrentStock += (decimal)detail.Qty;
                    _context.Entry(itemMasters).State = EntityState.Modified;
                }
            }

            if (saleMaster.salesDetails != null)
            {
                foreach (var detail in saleMaster.salesDetails)
                {
                    if (detail.ItemId > 0)
                    {
                        var oldDetail = await _context.SalesDet.AsNoTracking().FirstOrDefaultAsync(d => d.Id == detail.Id);
                        if (oldDetail != null)
                        {
                            itemMaster = _context.Items?.Find(detail.ItemId);
                            itemMaster.CurrentStock = itemMaster.CurrentStock + (decimal)oldDetail.Qty;
                            _context.Entry(itemMaster).State = EntityState.Modified;
                        }
                    }
                }

                foreach (var detail in saleMaster.salesDetails)
                {
                    if (detail.ItemId > 0)
                    {
                        itemMaster = _context.Items?.Find(detail.ItemId);
                        itemMaster.CurrentStock = itemMaster.CurrentStock - (decimal)detail.Qty;
                        _context.Entry(itemMaster).State = EntityState.Modified;
                    }
                }
            }

            _context.SalesMas.Update(saleMaster);
            await _context.SaveChangesAsync();

            return saleMaster.Id;
        }

        public async Task<bool> DeleteSalesDataAsync(int id)
        {
            try
            {
                var saleDet = await GetSaleDetailDataByIdAsync(id);
                var saleMas = await GetSaleMasterDataByIdAsync(id);

                _context.SalesDet.RemoveRange(saleDet);
                var itemMaster = new Item();
                foreach (var detail in saleMas.salesDetails)
                {
                    if (detail.ItemId > 0)
                    {
                        itemMaster = _context.Items?.Find(detail.ItemId);
                        itemMaster.CurrentStock = itemMaster.CurrentStock + (decimal)detail.Qty;
                        _context.Entry(itemMaster).State = EntityState.Modified;
                    }
                }
                _context.SalesMas.Remove(saleMas);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<SaleDto>> GetSaleDataListAsync()
        {
            var saleData = await _context.SalesMas
                .Include(sm => sm.PartyMaster)
                .Include(sm => sm.salesDetails)
                    .ThenInclude(sd => sd.ItemMaster)
                .Select(sm => new SaleDto
                {
                    Id = sm.Id,
                    SaleDate = sm.SalesDate,
                    DueDays = sm.DueDays,
                    DueDate = sm.DueDate,
                    PartyMasterId = sm.PartyMasterId,
                    PartyName = sm.PartyMaster.Name,
                    Qty = sm.salesDetails.Sum(sd => sd.Qty),
                    Amount = sm.salesDetails.Sum(sd => sd.Qty * sd.ItemMaster.Price),
                    salesDetails = sm.salesDetails.Select(sd => new SaleDetailDto
                    {
                        Id = sd.Id,
                        ItemId = sd.ItemId,
                        SalesMasterId = sd.SalesMasterId,
                        Qty = sd.Qty
                    }).ToList()
                })
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            return saleData;
        }
    }
}
