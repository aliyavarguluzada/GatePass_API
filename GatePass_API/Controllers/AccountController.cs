using GatePass_API.Core.Requests;
using GatePass_API.Data;
using GatePass_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace GatePass_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<ActionResult> Register(RegisterRequest request)
        {
            var user = new User
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
            };

            using (SHA256 sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
                user.Password = bytes;
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok();
        }
    }
}
