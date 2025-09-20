using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Repositories;
using iText.Commons.Bouncycastle.Asn1.X509;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace AllInOneProject.Services
{
    public interface ICartService
    {
        Task<ServiceResponse<List<CartDto>>> GetCartItemsAsync(string userId);
        Task ClearCartAsync(string userId);
    }

    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<ServiceResponse<List<CartDto>>> GetCartItemsAsync(string userId)
        {
            try
            {
                var result = await _cartRepository.GetCartItemsAsync(userId);
                if (result == null || result.Count == 0)
                {
                    return new ServiceResponse<List<CartDto>>
                    {
                        Success = false,
                        Message = "No items found",
                        Data = new List<CartDto>()
                    };
                }

                var cartDtos = new List<CartDto>();
                foreach (var cart in result)
                {
                    cartDtos.Add(new CartDto { Id = cart.Id, ItemId = cart.ItemId, UserId = cart.UserId, Quantity = cart.Quantity,
                        item = new ItemDto
                        {
                            Id = cart.item.Id,
                            Name = cart.item.Name,
                            Price = cart.item.Price
                        }
                    });
                }

                return new ServiceResponse<List<CartDto>>
                {
                    Success = true,
                    Message = "Cart items retrieved successfully",
                    Data = cartDtos
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<CartDto>>
                {
                    Success = true,
                    Message = $"Error in retrieving cart items: {ex.Message}"
                };
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            try
            {
                await _cartRepository.ClearCartAsync(userId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
