using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RetailAPI.Models
{
    public class PurchaseRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public int OwnerId { get; set; }

        public string Status { get; set; } = "Draft";
        public DateTime CreatedAt { get; set; }
        public List<PurchaseRequestItem> Items { get; set; } = new();
    }
}
