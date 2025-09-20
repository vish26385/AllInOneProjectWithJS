using AllInOneProject.Data;
using AllInOneProject.Models;
using AllInOneProject.Services;
using Microsoft.EntityFrameworkCore;

namespace AllInOneProject.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> PlaceOrderAsync(string userId, List<Cart> cartItems);
        Task<Order> GetOrderByIdAsync(int orderId);
    }
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> PlaceOrderAsync(string userId, List<Cart> cartItems)
        {
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(c => c.item.Price),
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ItemId = c.ItemId,
                    Quantity = c.Quantity,
                    Price = c.item.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}
