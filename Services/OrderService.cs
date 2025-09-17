using AllInOneProject.Data;
using AllInOneProject.DTOs;
using AllInOneProject.Models;
using AllInOneProject.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace AllInOneProject.Services
{
    public interface IOrderService
    {
        Task<ServiceResponse<OrderDto>> PlaceOrderAsync(int userId, List<CartDto> cartItems);
        Task<ServiceResponse<OrderDto>> GetOrderByIdAsync(int orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepository;
        public OrderService(ApplicationDbContext context, IOrderRepository orderRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
        }

        public async Task<ServiceResponse<OrderDto>> PlaceOrderAsync(int userId, List<CartDto> cartItems)
        {
            try
            {
                var lstCartItems = new List<Cart>();
                foreach (var cartItem in cartItems)
                {
                    lstCartItems.Add(new Cart
                    {
                        Id = cartItem.Id,
                        ItemId = cartItem.ItemId,
                        UserId = cartItem.UserId,
                        Quantity = cartItem.Quantity,
                        item = new Item
                        {
                            Id = cartItem.item.Id,
                            Name = cartItem.item.Name,
                            Price = cartItem.item.Price
                        }
                    });
                }
                var result = await _orderRepository.PlaceOrderAsync(userId, lstCartItems);
                var orderDto = new OrderDto
                {
                    Id = result.Id,
                    UserId = result.UserId,
                    OrderDate = result.OrderDate,
                    TotalAmount = result.TotalAmount,
                    OrderItems = result.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        ItemId = oi.ItemId,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        Item = oi.Item == null ? null : new ItemDto
                        {
                            Id = oi.Item.Id,
                            Name = oi.Item.Name,
                            Price = oi.Item.Price
                        }
                    }).ToList()
                };
                return new ServiceResponse<OrderDto>
                {
                    Success = true,
                    Message = "Order placed successfully",
                    Data = orderDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<OrderDto>
                {
                    Success = true,
                    Message = $"Error in placing order: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<OrderDto>> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var result = await _orderRepository.GetOrderByIdAsync(orderId);
                if (result == null)
                {
                    return new ServiceResponse<OrderDto>
                    {
                        Success = false,
                        Message = "Order not found"
                    };
                }
                var orderDto = new OrderDto
                {
                    Id = result.Id,
                    UserId = result.UserId,
                    OrderDate = result.OrderDate,
                    TotalAmount = result.TotalAmount,
                    OrderItems = result.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        ItemId = oi.ItemId,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        Item = oi.Item == null ? null : new ItemDto
                        {
                            Id = oi.Item.Id,
                            Name = oi.Item.Name,
                            Price = oi.Item.Price
                        }
                    }).ToList()
                };
                return new ServiceResponse<OrderDto>
                {
                    Success = true,
                    Message = "Order placed successfully",
                    Data = orderDto
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<OrderDto>
                {
                    Success = true,
                    Message = $"Error in getting order: {ex.Message}"
                };
            }
        }
    }
}
