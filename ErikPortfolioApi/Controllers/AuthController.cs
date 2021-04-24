using ErikPortfolioApi.exceptions;
using ErikPortfolioApi.Model;
using ErikPortfolioApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ErikPortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid payload");
            }

            try
            {
                var token = await _authService.Login(request.Username, request.Password);
                return Ok(new { token });
            }
            catch (FailedLoginException e)
            {
                return Unauthorized(e.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid payload");
            }

            await _authService.AddLogin(request.Username, request.Password);

            return Ok();
        }
    }
}
