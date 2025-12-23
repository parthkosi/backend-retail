using System.ComponentModel.DataAnnotations;

namespace RetailAPI.DTOs
{
    public class PrCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0m;
        public List<PrItemDto> Items { get; set; } = new List<PrItemDto>();
    }
}