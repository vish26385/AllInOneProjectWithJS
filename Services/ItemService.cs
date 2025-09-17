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

        public async Task<ServiceResponse<ItemDto>> InsertItemAsync(ItemRequest request)
        {
            try
            {
                // Map DTO → Model(Entity)
                var item = new Item
                {
                    Name = request.Name,
                    Price = request.Price ?? 0
                };

                var result = await _repository.InsertItemAsync(item);

                var dto = new ItemDto { Id = result.Id, Name = result.Name, Price = result.Price };

                return new ServiceResponse<ItemDto>
                {
                    Success = true,
                    Message = "Item inserted successfully",
                    Data = dto  // e.g., number of rows affected or new ItemId
                };
            }
            catch (SqlException ex) // database-specific errors
            {
                // log ex (Serilog, NLog, etc.)
                return new ServiceResponse<ItemDto>
                {
                    Success = false,
                    Message = "A database error occurred while inserting the item."
                };
            }
            catch (Exception ex) // fallback for unexpected errors
            {
                // log ex
                return new ServiceResponse<ItemDto>
                {
                    Success = false,
                    Message = "An unexpected error occurred while inserting the item."
                };
            }
        }

        public async Task<ServiceResponse<bool>> UpdateItemAsync(ItemRequest request)
        {
            if (!await _repository.ExistsAsync(request.Id))
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Item with Id {request.Id} not found."
                };
            }
            
            try
            {
                // Map DTO → Model(Entity)
                var item = new Item
                {
                    Id = request.Id,
                    Name = request.Name,
                    Price = request.Price ?? 0
                };

                var result = await _repository.UpdateItemAsync(item);

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Item updated successfully",
                    Data = result  // e.g., number of rows affected
                };
            }
            catch (Exception ex)
            {
                // log exception here
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating item: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteItemAsync(int id)
        {
            try
            {
                var result = await _repository.DeleteItemAsync(id);

                if (!result)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = $"Item with Id {id} not found."
                    };
                }

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Item deleted successfully",
                    Data = result  // e.g., number of rows affected
                };
            }
            catch (Exception ex)
            {
                // log exception
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting item: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<List<ItemDto>>> GetAllItemsAsync()
        {
            var items = await _repository.GetAllItemsAsync();

            if (items == null || items.Count == 0)
            {
                return new ServiceResponse<List<ItemDto>>
                {
                    Success = false,
                    Message = "No items found",
                    Data = new List<ItemDto>()
                };
            }

            var itemDTOs = new List<ItemDto>();
            foreach (var item in items)
            {
                itemDTOs.Add(new ItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price
                });
            }

            return new ServiceResponse<List<ItemDto>>
            {
                Success = true,
                Message = "Items retrieved successfully",
                Data = itemDTOs
            };
        }

        public async Task<ServiceResponse<ItemDto>> GetItemByIdAsync(int id)
        {
            var result = await _repository.GetItemByIdAsync(id);
            if (result == null)
            {
                return new ServiceResponse<ItemDto>
                {
                    Success = false,
                    Message = "No items found"
                };
            }
            var dto = new ItemDto{ Id = result.Id, Name = result.Name, Price = result.Price};
            return new ServiceResponse<ItemDto>
            {
                Success = true,
                Message = "Item retrieved successfully",
                Data = dto
            };
        }

        public async Task<ServiceResponse<List<ItemDto>>> GetUserCartItemsAsync(int userId)
        {
            var cartItems = await _repository.GetUserCartItemsAsync(userId);

            if (cartItems == null || cartItems.Count == 0)
            {
                return new ServiceResponse<List<ItemDto>>
                {
                    Success = false,
                    Message = "No cart items found",
                    Data = new List<ItemDto>()
                };
            }

            var cartItemDTOs = new List<ItemDto>();
            foreach (var item in cartItems)
            {
                cartItemDTOs.Add(new ItemDto { Id = item.Id, Name = item.Name, Price = item.Price });
            }
            return new ServiceResponse<List<ItemDto>>
            {
                Success = true,
                Message = "Cart items retrieved successfully",
                Data = cartItemDTOs
            };
        }

        public async Task<ServiceResponse<CartDto>> AddToCartAsync(int itemId, int userId)
        {
            try
            {
                var cartItem = new Cart
                {
                    ItemId = itemId,
                    UserId = userId,
                    Quantity = 1
                };

                var result = await _repository.AddToCartAsync(cartItem);
                if (result == null)
                {
                    return new ServiceResponse<CartDto>
                    {
                        Success = false,
                        Message = "Failed to add item to cart"
                    };
                }

                var cartDtos = new CartDto { Id = result.Id, ItemId = result.ItemId, UserId = result.UserId }; 

                return new ServiceResponse<CartDto>
                {
                    Success = true,
                    Message = "Item added to cart successfully",
                    Data = cartDtos  // e.g., number of rows affected
                };
            }
            catch (Exception ex)
            {
                // log exception here
                return new ServiceResponse<CartDto>
                {
                    Success = false,
                    Message = $"Error adding item to cart: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> RemoveFromCartAsync(int itemId, int userId)
        {
            try
            {
                var result = await _repository.RemoveFromCartAsync(itemId, userId);
                if (!result)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Item not found in cart."
                    };
                }
                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Item removed from cart successfully",
                    Data = result // e.g., number of rows affected
                };
            }
            catch (Exception ex)
            {
                // log exception here
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error removing item from cart: {ex.Message}"
                };
            }
        }
    }
}
