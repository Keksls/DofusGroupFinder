using Zaapix.Application.Services;
using Zaapix.Domain.DTO.Requests;
using Microsoft.AspNetCore.Mvc;
using Zaapix.Api.Configuration;
using Microsoft.Extensions.Options;

namespace Zaapix.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UpdateOptions _updateOptions;

        public AuthController(IAuthService authService, IOptions<UpdateOptions> updateOptions)
        {
            _authService = authService;
            _updateOptions = updateOptions.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok(new
            {
                version = _updateOptions.Version,
                url = _updateOptions.Url
            });
        }
    }
}