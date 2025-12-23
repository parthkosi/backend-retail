using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;
using RetailAPI.DTOs;
using RetailAPI.Models;
using System.Security.Claims;

namespace RetailAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseRequestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchaseRequestController(AppDbContext context)
        {
            _context = context;
        }

        // =================== PR CREATE ===================
        [HttpPost("create")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Create([FromBody] PrCreateDto dto)
        {
            var userId = GetUserId();

            var pr = new PurchaseRequest
            {
                Title = dto.Title,
                Description = dto.Description,
                Amount = dto.Amount,
                OwnerId = userId,
                Status = "Draft",
                CreatedAt = DateTime.Now,
                Items = dto.Items.Select(i => new PurchaseRequestItem
                {
                    ItemName = i.ItemName,
                    HsnNumber = i.HsnNumber,
                    ItemCode = i.ItemCode,
                    Quantity = i.Quantity,
                    CostPrice = i.CostPrice,
                    Mrp = i.Mrp
                }).ToList()
            };

            _context.PurchaseRequests.Add(pr);
            await _context.SaveChangesAsync();

            AddAudit("PR Created", userId, pr.Id, pr.Title);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = pr.Id }, pr);
        }

        // =================== PR READ ===================
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var pr = await _context.PurchaseRequests
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            return pr == null ? NotFound() : Ok(pr);
        }

        [HttpGet("list")]
        [Authorize]
        public async Task<IActionResult> List([FromQuery] string? status, [FromQuery] bool onlyMine = false)
        {
            var q = _context.PurchaseRequests
                .Include(p => p.Items)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                q = q.Where(p => p.Status == status);

            if (onlyMine)
            {
                var userId = GetUserId();
                q = q.Where(p => p.OwnerId == userId);
            }

            return Ok(await q.OrderByDescending(p => p.CreatedAt).ToListAsync());
        }

        // =================== PR EDIT ===================
        [HttpPut("edit/{id}")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Edit(int id, [FromBody] PrCreateDto dto)
        {
            var userId = GetUserId();
            var pr = await _context.PurchaseRequests
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pr == null) return NotFound();
            if (pr.OwnerId != userId && !User.IsInRole("Admin")) return Forbid();
            if (pr.Status != "Draft" && pr.Status != "Submitted")
                return BadRequest("Cannot edit after approval.");

            pr.Title = dto.Title;
            pr.Description = dto.Description;
            pr.Amount = dto.Amount;

            _context.PurchaseRequestItems.RemoveRange(pr.Items);

            pr.Items = dto.Items.Select(i => new PurchaseRequestItem
            {
                ItemName = i.ItemName,
                HsnNumber = i.HsnNumber,
                ItemCode = i.ItemCode,
                Quantity = i.Quantity,
                CostPrice = i.CostPrice,
                Mrp = i.Mrp
            }).ToList();

            await _context.SaveChangesAsync();
            AddAudit("PR Edited", userId, pr.Id);
            await _context.SaveChangesAsync();

            return Ok(pr);
        }

        // =================== PR DELETE ===================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var pr = await _context.PurchaseRequests
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pr == null) return NotFound();
            if (pr.OwnerId != userId && !User.IsInRole("Admin")) return Forbid();
            if (pr.Status != "Draft") return BadRequest();

            _context.PurchaseRequestItems.RemoveRange(pr.Items);
            _context.PurchaseRequests.Remove(pr);

            AddAudit("PR Deleted", userId, pr.Id);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =================== PR SUBMIT ===================
        [HttpPost("submit/{id}")]
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Submit(int id)
        {
            var userId = GetUserId();
            var pr = await _context.PurchaseRequests.FindAsync(id);

            if (pr == null) return NotFound();
            if (pr.OwnerId != userId && !User.IsInRole("Admin")) return Forbid();
            if (pr.Status != "Draft") return BadRequest("Only Draft PRs can be submitted.");

            pr.Status = "Submitted";
            await _context.SaveChangesAsync();

            AddAudit("PR Submitted", userId, pr.Id);
            await _context.SaveChangesAsync();

            return Ok(pr);
        }

        // =================== PR APPROVE ===================
        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApproveDto dto)
        {
            var pr = await _context.PurchaseRequests.FindAsync(id);
            if (pr == null) return NotFound();
            if (pr.Status != "Submitted") return BadRequest();

            pr.Status = "Approved";
            await _context.SaveChangesAsync();

            AddAudit("PR Approved", GetUserId(), pr.Id, dto.Comment);
            await _context.SaveChangesAsync();

            return Ok(pr);
        }

        // =================== REJECT ===================
        [HttpPost("reject/{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Reject(int id, [FromBody] ApproveDto dto)
        {
            var pr = await _context.PurchaseRequests.FindAsync(id);
            if (pr == null) return NotFound();
            if (pr.Status != "Submitted") return BadRequest();

            pr.Status = "Rejected";
            await _context.SaveChangesAsync();

            AddAudit("PR Rejected", GetUserId(), pr.Id, dto.Comment);
            await _context.SaveChangesAsync();

            return Ok(pr);
        }

        // =================== PR MARK PAID + CREATE STOCK ===================
        [HttpPost("{id}/mark-paid")]
        [Authorize(Roles = "Finance,Admin")]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var pr = await _context.PurchaseRequests
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pr == null) return NotFound();
            if (pr.Status != "Approved") return BadRequest("Only Approved PRs allowed.");

            bool stockExists = await _context.Stocks
                .AnyAsync(s => s.PurchaseRequestItem!.PrId == pr.Id);

            if (stockExists)
                return BadRequest("Stock already created.");

            pr.Status = "Paid";

            foreach (var item in pr.Items)
            {
                _context.Stocks.Add(new Stock
                {
                    CreatedFromPrItemId = item.Id,
                    ItemName = item.ItemName,
                    ItemCode = item.ItemCode,
                    HsnNumber = item.HsnNumber,
                    Quantity = item.Quantity,
                    CostPrice = item.CostPrice,
                    Mrp = item.Mrp,
                    Batch = GenerateBatch(),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }

            AddAudit("PR Paid → Stock Created", GetUserId(), pr.Id);
            await _context.SaveChangesAsync();

            return Ok(new { message = "PR paid and stock created" });

        }

        // =================== HELPERS ===================
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                      ?? User.FindFirst("id")
                      ?? User.FindFirst("sub");

            if (claim == null || !int.TryParse(claim.Value, out int id))
                throw new Exception("Invalid UserId");

            return id;
        }

        private void AddAudit(string action, int userId, int prId, string? details = null)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                Action = action,
                PerformedBy = userId,
                PrId = prId,
                Details = details
            });
        }

        private string GenerateBatch()
        {
            return $"B{DateTime.UtcNow:yyMMdd}-{Guid.NewGuid().ToString("N")[..5].ToUpper()}";
        }
    }
}
