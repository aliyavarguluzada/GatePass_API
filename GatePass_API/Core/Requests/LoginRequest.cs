namespace GatePass_API.Core.Requests
{
    public class LoginRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
