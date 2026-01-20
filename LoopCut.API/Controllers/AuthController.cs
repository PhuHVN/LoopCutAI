using LoopCut.Application.DTOs;
using LoopCut.Application.DTOs.AccountDtos;
using LoopCut.Application.DTOs.LoginDtos;
using LoopCut.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LoopCut.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService service;
        public AuthController(IAuthService service)
        {
            this.service = service;
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary ="Register a new user")]
        public async Task<IActionResult> Register(AccountRequest request)
        {
            var result = await service.Register(request);
            return Ok(ApiResponse<AccountResponse>.OkResponse(result, "Register succesful!", "200"));
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login with email and password")]
        public async Task<IActionResult> LoginEmail(LoginRequest request)
        {
            var result = await service.LoginEmail(request);
            return Ok(ApiResponse<AuthResponse>.OkResponse(result, "Login successful!", "200"));
        }

        [HttpPost("login-google")]
        [SwaggerOperation(Summary = "Login with Google account")]
        public async Task<IActionResult> LoginGoogle(LoginGoogleRequest request)
        {
            var result = await service.LoginGoogle(request);
            return Ok(ApiResponse<AuthResponse>.OkResponse(result, "Login with Google successful!", "200"));
        }

        [HttpGet("current-user")]
        [SwaggerOperation(Summary = "Get current logged-in user")]
        public async Task<IActionResult> CurrentUser()
        {
            var result = await service.CurrentUser();
            return Ok(ApiResponse<AccountResponse>.OkResponse(result, "Current user fetched successfully!", "200"));
        }
    }
}
