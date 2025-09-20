using AllInOneProject.Controllers;
using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AllInOneProject.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        public ItemRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Items.AnyAsync(x => x.Id == id);
        }

        public async Task<Item> InsertItemAsync(Item item)
        {
            await using var con = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand("sp_InsertItem", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = item.Name;
            cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = item.Price;
            // Add output parameter for new Id
            var idParam = cmd.Parameters.Add("@Id", SqlDbType.Int);
            idParam.Direction = ParameterDirection.Output;
            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            // Set the generated Id back on the entity
            item.Id = (int)idParam.Value;

            return item;
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            // Attach and mark entity as modified
            _context.Entry(item).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;            
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await GetItemByIdAsync(id);
            if (item == null)
                return false;
            _context.Items.Remove(item);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            var items = new List<Item>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetAllItems", con);
            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                items.Add(new Item
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString() ?? "",
                    Price = Convert.ToDecimal(reader["Price"]),
                    CurrentStock = Convert.ToDecimal(reader["CurrentStock"])
                });
            }

            return items;
        }

        public async Task<Item?> GetItemByIdAsync(int id)
        {
            return await _context.Items.FindAsync(id);
        }

        public async Task<List<Item>> GetUserCartItemsAsync(int userId)
        {
            var cartItems = await _context.Carts
                .Include(c => c.item)
                .Where(c => c.UserId == userId)
                .Select(c => c.item)
                .ToListAsync();

            return cartItems;
        }

        public async Task<Cart> AddToCartAsync(Cart cartItem)
        {
            _context.Carts.Add(cartItem);
            await _context.SaveChangesAsync();

            return cartItem; // entity with generated Id
        }

        public async Task<bool> RemoveFromCartAsync(int itemId, int userId)
        {
            var cartItem = await _context.Carts.FirstOrDefaultAsync(x => x.ItemId == itemId && x.UserId == userId);
            if (cartItem == null) return false;

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
