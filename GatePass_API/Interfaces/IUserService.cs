using GatePass_API.Core;
using GatePass_API.Core.Requests;
using GatePass_API.Core.Responses;

namespace GatePass_API.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult<AddUserResponse>> AddUser(AddUserRequests request);
    }
}
