using Microsoft.AspNetCore.Mvc;

namespace GatePass_API.Controllers
{
    public class IdentityController : ControllerBase
    {
        private const string TokenSecret = "ThisismySecretKey";
        public static readonly TimeSpan TokenLifeTime = TimeSpan.FromHours(1);

        //[HttpPost("token")]
        //public IActionResult GenerateToken([FromBody]TokenGenerationRequest request)
        //{
        //    return Ok();
        //}
    }
}
