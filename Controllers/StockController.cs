using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;
using RetailAPI.Models;
using RetailAPI.DTOs;


namespace RetailAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Manager,Admin")]
	public class StockController : ControllerBase
	{
		private readonly AppDbContext _context;

		public StockController(AppDbContext context)
		{
			_context = context;
		}

		// =================== GET STOCK ===================
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var stock = await _context.Stocks
				.OrderByDescending(s => s.CreatedAt)
				.ToListAsync();

			return Ok(stock);
		}

		// =================== SET SALE PRICE ===================

	[HttpPut("{id}/sale-price")]
	public async Task<IActionResult> SetSalePrice(int id,[FromBody] SetSalePriceDto dto)
	{
		if (dto == null)
			return BadRequest("Invalid request body.");

		var stock = await _context.Stocks.FindAsync(id);
		if (stock == null) return NotFound();

		if (dto.SalePrice <= 0)
			return BadRequest("Sale price must be greater than zero.");

		if (dto.SalePrice < stock.CostPrice)
			return BadRequest("Sale price cannot be below cost price.");

		stock.SalePrice = dto.SalePrice;
		stock.UpdatedAt = DateTime.Now;

		await _context.SaveChangesAsync();

		return Ok(stock);
	}


}
}
