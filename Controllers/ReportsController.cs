using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;

namespace RetailAPI.Controllers
{
    [Authorize(Roles = "Manager")]
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // REPORT 1: PURCHASE REQUEST LIFECYCLE
        // Raised → Approved → Paid
        // =====================================================
        [HttpGet("pr-lifecycle")]
        public async Task<IActionResult> GetPrLifecycleReport()
        {
            var report = await _context.PurchaseRequests
                .AsNoTracking()
                .Select(pr => new
                {
                    PrId = pr.Id,
                    pr.Title,
                    pr.Amount,
                    pr.Status,

                    // Raised
                    RaisedAt = pr.CreatedAt,

                    // Approved (from AuditLog)
                    ApprovedAt = _context.AuditLogs
                        .Where(a => a.PrId == pr.Id && a.Action == "PR Approved")
                        .Select(a => (DateTime?)a.CreatedAt)
                        .FirstOrDefault(),

                    // Paid (from AuditLog)
                    PaidAt = _context.AuditLogs
                        .Where(a => a.PrId == pr.Id && a.Action == "PR Paid → Stock Created")
                        .Select(a => (DateTime?)a.CreatedAt)
                        .FirstOrDefault(),

                    ItemCount = pr.Items.Count
                })
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();

            return Ok(report);
        }

        // =====================================================
        // REPORT 2: CURRENT STOCK (BATCH WISE)
        // =====================================================
        [HttpGet("stock-batch")]
        public async Task<IActionResult> GetStockBatchWiseReport()
        {
            var stockReport = await _context.Stocks
                .AsNoTracking()
                .Select(s => new
                {
                    s.Id,

                    s.ItemName,
                    s.ItemCode,
                    s.HsnNumber,

                    Batch = s.Batch,

                    s.Quantity,
                    s.Mrp,
                    s.CostPrice,
                    s.SalePrice,

                    StockValue = s.Quantity * s.CostPrice,

                    CreatedFromPrItemId = s.CreatedFromPrItemId,
                    CreatedAt = s.CreatedAt
                })
                .OrderByDescending(s => s.Batch)
                .ThenBy(s => s.ItemName)
                .ToListAsync();

            return Ok(stockReport);
        }
    }
}
