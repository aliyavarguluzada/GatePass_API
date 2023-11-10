using GatePass_API.Core;
using GatePass_API.Core.Requests;
using GatePass_API.Core.Responses;
using GatePass_API.Data;
using GatePass_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GatePass_API.Interfaces
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<LoginResponse>> Login(LoginRequest request)
        {
            var user = await _context
                            .Users
                            .Where(c => c.Email == request.Email)
                            .FirstOrDefaultAsync();

            if (user is null)
                return ServiceResult<LoginResponse>.ERROR("", "Belə bir user mövcud deyil");


            using (SHA256 sha256Hash = SHA256.Create())
            {

                byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

                if (!user.Password.SequenceEqual(hash))
                    return ServiceResult<LoginResponse>.ERROR("", "Şifrə yanlışdır");

            }

            var response = new LoginResponse();

            return ServiceResult<LoginResponse>.OK(response);
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

                using (SHA256 sha256Hash = SHA256.Create())
                {
                    var hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
                    user.Password = hash;
                }

                _context.Users.Add(user);
                _context.SaveChanges();

                transaction.CommitAsync();

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
    }
}
