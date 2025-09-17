﻿using AllInOneProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public List<OrderItemDto> OrderItems { get; set; }
    }
}
