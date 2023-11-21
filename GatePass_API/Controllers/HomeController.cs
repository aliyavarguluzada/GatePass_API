using GatePass_API.Core.Requests;
using GatePass_API.Data;
using GatePass_API.DTOs;
using GatePass_API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GatePass_API.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, IConfiguration configuration, IUserService userService)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost("Users")]
        public async Task<ActionResult> Users(GetUserRequest request)
        {
            var user = await _context
                .Users
                .Where(c => c.Email == request.Email)
                .Select(c => new UserDto
                {
                    UserId = c.Id,
                    UserName = c.Name,
                    UserSurname = c.Surname,
                    UserEmail = c.Email
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
