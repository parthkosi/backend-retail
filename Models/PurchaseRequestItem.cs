using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailAPI.Models
{
    public class PurchaseRequestItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(PurchaseRequest))]
        public int PrId { get; set; }
        public PurchaseRequest? PurchaseRequest { get; set; }

        [Required]
        public string ItemName { get; set; } = string.Empty;

        public string HsnNumber { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;

        public decimal CostPrice { get; set; }
        public decimal Mrp { get; set; }
    }
}
