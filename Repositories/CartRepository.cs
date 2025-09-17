using AllInOneProject.Data;
using AllInOneProject.Models;
using AllInOneProject.Services;
using Microsoft.EntityFrameworkCore;
using System;

namespace AllInOneProject.Repositories
{
    public interface ICartRepository
    {
        Task<List<Cart?>> GetCartItemsAsync(int userId);
        Task ClearCartAsync(int userId);
    }

    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cart?>> GetCartItemsAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.item)  // so you can access Item details like Price, Name
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var items = _context.Carts.Where(c => c.UserId == userId);
            _context.Carts.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
