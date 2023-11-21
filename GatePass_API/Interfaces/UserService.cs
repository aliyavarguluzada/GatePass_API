using GatePass_API.Core;
using GatePass_API.Core.Requests;
using GatePass_API.Core.Responses;
using GatePass_API.Data;
using GatePass_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GatePass_API.Interfaces
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public UserService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResult<LoginResponse>> Login(LoginRequest request)
        {
            try
            {

                var user = await _context
                                .Users
                                .Where(c => c.Email == request.Email)
                                .FirstOrDefaultAsync();

                if (user is null)
                    return ServiceResult<LoginResponse>.ERROR("", "Belə bir user mövcud deyil");


                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                    return ServiceResult<LoginResponse>.ERROR("", "Password yanlışdır");

                var response = new LoginResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,


                };
                string token = TokenCreator(request);

                return ServiceResult<LoginResponse>.OK(response);

            }
            catch (Exception)
            {
                return ServiceResult<LoginResponse>.ERROR("", "Uçdu");
            }
        }

        public async Task<ServiceResult<RegisterResponse>> Register(RegisterRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new User
                {
                    Name = request.Name,
                    Surname = request.Surname,
                    Email = request.Email
                };


                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                user.Password = passwordHash;

                _context.Users.Add(user);
                _context.SaveChanges();

                await transaction.CommitAsync();

                var response = new RegisterResponse
                {
                    Id = request.Id,
                    Email = request.Email,
                    Name = request.Name,
                    Surname = request.Surname
                };

                return ServiceResult<RegisterResponse>.OK(response);
            }
            catch (Exception)
            {
                transaction.Rollback();
                return ServiceResult<RegisterResponse>.ERROR("", "Uçdu");
            }
        }

        public string TokenCreator(LoginRequest request)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, request.Email ),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}
