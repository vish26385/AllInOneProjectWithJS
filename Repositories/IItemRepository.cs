using AllInOneProject.Controllers;
using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Repositories
{
    public interface IItemRepository
    {
        Task<int> InsertItemAsync(ItemRequest request);
        Task<int> UpdateItemAsync(ItemRequest request);
        Task<int> DeleteItemAsync(int id);
        Task<Item?> GetItemByIdAsync(int id);
        Task<List<Item>> GetAllItemsAsync();
        Task<List<Item>> GetUserCartItemsAsync(int userId);
        Task<int> AddToCartAsync(int itemId, int userId);
        Task<int> RemoveFromCartAsync(int itemId, int userId);
    }
}
