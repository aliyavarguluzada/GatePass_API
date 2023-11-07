using GatePass_API.Core;
using GatePass_API.Core.Requests;
using GatePass_API.Core.Responses;
using GatePass_API.Data;
using GatePass_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;

namespace GatePass_API.Interfaces
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResult<AddUserResponse>> AddUser(AddUserRequests request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
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
                var response = new AddUserResponse
                {
                    Id = request.Id,
                    Email = request.Email,
                    Name = request.Name,
                    Surname = request.Surname
                };

                return ServiceResult<AddUserResponse>.OK(response);
            }
            catch (Exception)
            {
                transaction.Rollback();
                return ServiceResult<AddUserResponse>.ERROR("", "Uçdu");
            }
        }
    }
}
