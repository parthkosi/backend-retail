using System.ComponentModel.DataAnnotations;

namespace RetailAPI.DTOs
{
    public class PrItemDto
    {
        public string ItemName { get; set; } = string.Empty;
        public string HsnNumber { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;

        public decimal CostPrice { get; set; }
        public decimal Mrp { get; set; }
    }
}