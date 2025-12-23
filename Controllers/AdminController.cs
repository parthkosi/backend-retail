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
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserListDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Role = u.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        // PUT: api/admin/change-role
        [HttpPut("change-role")]
        public async Task<IActionResult> ChangeRole(ChangeRoleDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found");

            user.Role = dto.NewRole;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Role updated successfully" });
        }
    }
}
