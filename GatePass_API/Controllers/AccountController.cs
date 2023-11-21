using Azure;
using GatePass_API.Core.Requests;
using GatePass_API.Data;
using GatePass_API.DTOs;
using GatePass_API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace GatePass_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context, IConfiguration configuration, IUserService userService)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
        }


        [HttpPost("Register")]
        public ActionResult Register(RegisterRequest request)
        {
            var result = _userService.Register(request);


            return Ok(result);
        }

        [HttpDelete("Delete")]
        public ActionResult DeleteUser(string name)
        {
            var user = _context
                .Users
                .Where(c => c.Name == name)
                .FirstOrDefault();


            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost("Login")]
        [Authorize]

        public ActionResult Login([FromBody] LoginRequest request)
        {
            var result = _userService.Login(request);


            return Ok(result);
        }



        [HttpGet("Users")]
        [Authorize]
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
