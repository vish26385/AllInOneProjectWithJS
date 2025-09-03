using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AllInOneProject.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
    
        public SaleRepository(IConfiguration configuration, ApplicationDbContext context)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
            _context = context;
        }

        public async Task<SalesMaster> GetSaleMasterDataByIdAsync(int id)
        {
            return await _context.SalesMas.FindAsync(id);
        }

        public async Task<SalesDetail> GetSaleDetailDataByIdAsync(int id)
        {
            return await _context.SalesDet.FirstOrDefaultAsync(x => x.SalesMasterId == id);
        }

        public async Task<int> SaveSalesDataAsync(SalesMasterRequest request)
        {
            if (request == null || request.SalesDetailRequests == null || !request.SalesDetailRequests.Any())
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
                            cmd.Parameters.AddWithValue("@SaleDate", request.SaleDate);
                            cmd.Parameters.AddWithValue("@DueDays", request.DueDays);
                            cmd.Parameters.AddWithValue("@DueDate", request.DueDate);
                            cmd.Parameters.AddWithValue("@PartyId", request.PartyId);
                            cmd.Parameters.Add("@SaleMasterId", SqlDbType.Int).Direction = ParameterDirection.Output;

                            await cmd.ExecuteNonQueryAsync();
                            saleMasterId = (int)cmd.Parameters["@SaleMasterId"].Value;
                        }

                        // Insert SaleDetails
                        foreach (var item in request.SalesDetailRequests)
                        {
                            using var cmdDetail = new SqlCommand("sp_InsertSaleDetail", con, (SqlTransaction)trn);
                            cmdDetail.CommandType = CommandType.StoredProcedure;
                            cmdDetail.Parameters.AddWithValue("@SaleMasterId", saleMasterId);
                            cmdDetail.Parameters.AddWithValue("@ItemId", item.ItemId);
                            cmdDetail.Parameters.AddWithValue("@Qty", item.Qty);

                            await cmdDetail.ExecuteNonQueryAsync();
                        }

                        await trn.CommitAsync();

                        //var saleMaster = new SalesMaster
                        //{
                        //    SalesDate = request.SaleDate,
                        //    DueDays = request.DueDays,
                        //    DueDate = request.DueDate,
                        //    PartyId = request.PartyId,
                        //    salesDetails = request.SalesDetailRequests.Select(d => new SalesDetail
                        //    {
                        //        itemId = d.ItemId,
                        //        Qty = d.Qty
                        //    }).ToList()
                        //};

                        //_context.SalesMas.Add(saleMaster);
                        //await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        await trn.RollbackAsync();
                        throw; // rethrow to service/controller
                    }
                }
            }

            return saleMasterId;
        }

        public async Task<int> UpdateSalesDataAsync(SalesMasterRequest request)
        {
            if (request == null || request.SalesDetailRequests == null || !request.SalesDetailRequests.Any())
                throw new ArgumentException("Invalid sale data.");

            try
            {
                var saleMaster = await GetSaleMasterDataByIdAsync(request.Id);
                if (saleMaster == null)
                {
                    throw new ArgumentException($"SaleMaster with Id={request.Id} not found.");
                }
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

                // Delete sale details in DeletedSaleDetailIds
                if (request.DeletedSaleDetailIds != null && request.DeletedSaleDetailIds.Any())
                {
                    var detailsToDelete = _context.SalesDet.Where(d => request.DeletedSaleDetailIds.Contains(d.Id));

                    _context.SalesDet.RemoveRange(detailsToDelete);
                }

                await _context.SaveChangesAsync();

                return saleMaster.Id;
            }
            catch
            {
                throw; // rethrow to service/controller
            }
        }

        public async Task<int> DeleteSalesDataAsync(int id)
        {
            try
            {
                var saleDet = await GetSaleDetailDataByIdAsync(id);
                var saleMas = await GetSaleMasterDataByIdAsync(id);

                _context.SalesDet.RemoveRange(saleDet);
                _context.SalesMas.Remove(saleMas);
                return await _context.SaveChangesAsync();
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
                    SaleDate = sm.SalesDate.ToString("yyyy-MM-dd"),
                    DueDays = sm.DueDays,
                    DueDate = sm.DueDate.ToString("yyyy-MM-dd"),
                    PartyId = sm.PartyId,
                    PartyName = sm.PartyMaster.Name,
                    Qty = sm.salesDetails.Sum(sd => sd.Qty),
                    Amount = sm.salesDetails.Sum(sd => sd.Qty * sd.ItemMaster.Price),
                    salesDetails = sm.salesDetails.Select(sd => new SaleDetailDto
                    {
                        Id = sd.Id,
                        itemId = sd.itemId,
                        SalesMasterId = sd.SalesMasterId,
                        Qty = sd.Qty
                    }).ToList()
                })
                .ToListAsync();

            return saleData;
        }
    }
}
