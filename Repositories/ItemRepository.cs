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

        public async Task<int> InsertItemAsync(ItemRequest request)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_InsertItem", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", request.Name);
            cmd.Parameters.AddWithValue("@Price", request.Price);
            await con.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> UpdateItemAsync(ItemRequest request)
        {
            var item = await _context.Items.FindAsync(request.Id);
            if (item != null)
            {
                item.Name = request.Name;
                item.Price = request.Price;
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<int> DeleteItemAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                return await _context.SaveChangesAsync();
            }
            return 0;
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
                    Price = Convert.ToDecimal(reader["Price"])
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

        public async Task<int> AddToCartAsync(int itemId, int userId)
        {
            var cartItem = new Cart
            {
                ItemId = itemId,
                UserId = userId
            };

            _context.Carts.Add(cartItem);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> RemoveFromCartAsync(int itemId, int userId)
        {
            var cartItem = await _context.Carts.FirstOrDefaultAsync(x => x.ItemId == itemId && x.UserId == userId);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
    }
}
