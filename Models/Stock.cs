using System.ComponentModel.DataAnnotations.Schema;

namespace RetailAPI.Models
{
    public class Stock
    {
        public int Id { get; set; }

        [ForeignKey("PurchaseRequestItem")]
        public int CreatedFromPrItemId { get; set; }
        public PurchaseRequestItem? PurchaseRequestItem { get; set; }

        public string ItemName { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string HsnNumber { get; set; } = string.Empty;

        public string Batch { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal Mrp { get; set; }
        public decimal CostPrice { get; set; }
        public decimal? SalePrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
