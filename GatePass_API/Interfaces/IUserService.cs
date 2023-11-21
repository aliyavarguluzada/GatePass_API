using GatePass_API.Core;
using GatePass_API.Core.Requests;
using GatePass_API.Core.Responses;

namespace GatePass_API.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<RegisterResponse>> Register(RegisterRequest request);

        Task<ServiceResult<LoginResponse>> Login(LoginRequest request);

    }
}
