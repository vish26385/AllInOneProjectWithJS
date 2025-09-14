using AllInOneProject.Controllers;
using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Services
{
    public interface IItemService
    {
        Task<ServiceResponse<Item>> InsertItemAsync(ItemRequest request);
        Task<ServiceResponse<bool>> UpdateItemAsync(ItemRequest request);
        Task<ServiceResponse<bool>> DeleteItemAsync(int id);
        Task<ServiceResponse<List<Item>>> GetAllItemsAsync();
        Task<ServiceResponse<Item>> GetItemByIdAsync(int id);
        Task<ServiceResponse<List<Item>>> GetUserCartItemsAsync(int userId);
        Task<ServiceResponse<int>> AddToCartAsync(int itemId, int userId);
        Task<ServiceResponse<int>> RemoveFromCartAsync(int itemId, int userId);
    }
}
