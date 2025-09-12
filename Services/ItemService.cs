using AllInOneProject.Controllers;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Repositories;
using Azure.Core;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AllInOneProject.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repository;

        public ItemService(IItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<int>> InsertItemAsync(ItemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Item name is required."
                };
            }

            if (request.Price <= 0)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Price must be greater than zero."
                };
            }

            // Map DTO → Model(Entity)
            var item = new Item
            {
                Name = request.Name,
                Price = request.Price
            };

            var result = await _repository.InsertItemAsync(item);

            return new ServiceResponse<int>
            {
                Success = true,
                Message = "Item inserted successfully",
                Data = result  // e.g., number of rows affected or new ItemId
            };
        }

        public async Task<ServiceResponse<int>> UpdateItemAsync(ItemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Item name is required."
                };
            }

            if (request.Price <= 0)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Price must be greater than zero."
                };
            }

            // Map DTO → Model(Entity)
            var item = new Item
            {
                Id = request.Id,
                Name = request.Name,
                Price = request.Price
            };

            var result = await _repository.UpdateItemAsync(item);

            if (result == 0)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "No items found",
                    Data = 0
                };
            }

            return new ServiceResponse<int>
            {
                Success = true,
                Message = "Item updated successfully",
                Data = result  // e.g., number of rows affected
            };
        }

        public async Task<ServiceResponse<int>> DeleteItemAsync(int id)
        {
            var result = await _repository.DeleteItemAsync(id);

            if (result == 0)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "No items found",
                    Data = 0
                };
            }

            return new ServiceResponse<int>
            {
                Success = true,
                Message = "Item deleted successfully",
                Data = result  // e.g., number of rows affected
            };
        }

        public async Task<ServiceResponse<List<Item>>> GetAllItemsAsync()
        {
            var items = await _repository.GetAllItemsAsync();

            if (items == null || items.Count == 0)
            {
                return new ServiceResponse<List<Item>>
                {
                    Success = false,
                    Message = "No items found",
                    Data = new List<Item>()
                };
            }

            return new ServiceResponse<List<Item>>
            {
                Success = true,
                Message = "Items retrieved successfully",
                Data = items
            };
        }
        public async Task<ServiceResponse<Item>> GetItemByIdAsync(int id)
        {
            var result = await _repository.GetItemByIdAsync(id);
            if (result == null)
            {
                return new ServiceResponse<Item>
                {
                    Success = false,
                    Message = "No items found",
                    Data = new Item()
                };
            }
            return new ServiceResponse<Item>
            {
                Success = true,
                Message = "Item retrieved successfully",
                Data = result
            };
        }

        public async Task<ServiceResponse<List<Item>>> GetUserCartItemsAsync(int userId)
        {
            var cartItems = await _repository.GetUserCartItemsAsync(userId);

            if (cartItems == null || cartItems.Count == 0)
            {
                return new ServiceResponse<List<Item>>
                {
                    Success = false,
                    Message = "No cart items found",
                    Data = new List<Item>()
                };
            }

            return new ServiceResponse<List<Item>>
            {
                Success = true,
                Message = "Cart items retrieved successfully",
                Data = cartItems
            };
        }
        public async Task<ServiceResponse<int>> AddToCartAsync(int itemId, int userId)
        {
            var result = await _repository.AddToCartAsync(itemId, userId);
            if (result == 0)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Failed to add item to cart",
                    Data = 0
                };
            }
            return new ServiceResponse<int>
            {
                Success = true,
                Message = "Item added to cart successfully",
                Data = result  // e.g., number of rows affected
            };
        }
        public async Task<ServiceResponse<int>> RemoveFromCartAsync(int itemId, int userId)
        {
            var result = await _repository.RemoveFromCartAsync(itemId, userId);
            if(result == 0)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = "Item not found",
                    Data = 0
                };
            }
            return new ServiceResponse<int>
            {
                Success = true,
                Message = "Item removed from cart successfully",
                Data = result // e.g., number of rows affected
            };
        }
    }
}
