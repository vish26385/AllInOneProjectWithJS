﻿using System.ComponentModel.DataAnnotations.Schema;

namespace AllInOneProject.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Item Item { get; set; }
    }
}
