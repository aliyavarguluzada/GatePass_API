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
                    Surname = user.Surname
                };

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
