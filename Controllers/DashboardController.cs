using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;

namespace RetailAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var approvedPRThisMonth = await _context.PurchaseRequests
                .Where(pr => pr.Status == "Paid")
                .CountAsync();

            var pendingPRs = await _context.PurchaseRequests
                .Where(pr => pr.Status == "Approved")
                .CountAsync();

            var totalPurchaseCostThisMonth = await _context.PurchaseRequests
                .Where(pr => pr.Status == "Paid")
                .SumAsync(pr => (decimal?)pr.Amount) ?? 0;

            var lowStockItems = await _context.PurchaseRequestItems
                .GroupBy(b => b.HsnNumber)
                .Where(g => g.Sum(b => b.Quantity) < 500)
                .CountAsync();

            var dashboardData = new
            {
                approvedPRThisMonth,
                pendingPRs,
                totalPurchaseCostThisMonth,
                lowStockItems
            };

            return Ok(dashboardData);
        }
    }
}