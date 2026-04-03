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
                    id = s.Id,

                    itemName = s.ItemName,
                    itemCode = s.ItemCode,
                    hsnNumber = s.HsnNumber,

                    batch = s.Batch,

                    quantity = s.Quantity,
                    mrp = s.Mrp,
                    costPrice = s.CostPrice,
                    salePrice = s.SalePrice,

                    stockValue = s.Quantity * s.CostPrice,

                    createdFromPrItemId = s.CreatedFromPrItemId,
                    createdAt = s.CreatedAt,

                    arrivedDate = s.CreatedAt
                })
                .OrderByDescending(s => s.batch)
                .ThenBy(s => s.itemName)
                .ToListAsync();

            return Ok(stockReport);
        }

        // REPORT 3: STOCK SUMMARY (ITEM WISE)
        // =====================================================
        [HttpGet("stock-summary")]
        public async Task<IActionResult> GetStockSummary()
        {
            var summary = await _context.Stocks
                .AsNoTracking()
                .GroupBy(s => new { s.ItemName, s.HsnNumber })
                .Select(g => new
                {
                    itemName = g.Key.ItemName,
                    hsnNumber = g.Key.HsnNumber,

                    totalQuantity = g.Sum(x => x.Quantity),

                    totalCostValue = g.Sum(x => x.Quantity * x.CostPrice),

                    batchCount = g.Count()
                })
                .OrderBy(x => x.itemName)
                .ToListAsync();

            return Ok(summary);
        }

    }
}
