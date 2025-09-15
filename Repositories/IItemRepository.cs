using AllInOneProject.Controllers;
using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Repositories
{
    public interface IItemRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<Item> InsertItemAsync(Item item);
        Task<bool> UpdateItemAsync(Item item);
        Task<bool> DeleteItemAsync(int id);
        Task<Item?> GetItemByIdAsync(int id);
        Task<List<Item>> GetAllItemsAsync();
        Task<List<Item>> GetUserCartItemsAsync(int userId);
        Task<Cart> AddToCartAsync(int itemId, int userId);
        Task<bool> RemoveFromCartAsync(int itemId, int userId);
    }
}
