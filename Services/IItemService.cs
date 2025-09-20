using AllInOneProject.Controllers;
using AllInOneProject.DTOs;
using AllInOneProject.Models;

namespace AllInOneProject.Services
{
    public interface IItemService
    {
        Task<ServiceResponse<ItemDto>> InsertItemAsync(ItemRequest request);
        Task<ServiceResponse<bool>> UpdateItemAsync(ItemRequest request);
        Task<ServiceResponse<bool>> DeleteItemAsync(int id);
        Task<ServiceResponse<List<ItemDto>>> GetAllItemsAsync();
        Task<ServiceResponse<ItemDto>> GetItemByIdAsync(int id);
        Task<ServiceResponse<List<ItemDto>>> GetUserCartItemsAsync(string userId);
        Task<ServiceResponse<CartDto>> AddToCartAsync(int itemId, string userId);
        Task<ServiceResponse<bool>> RemoveFromCartAsync(int itemId, string userId);
    }
}
