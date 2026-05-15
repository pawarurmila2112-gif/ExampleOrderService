using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ExampleOrderService.Dtos;
using ExampleOrderService.Services;

namespace ExampleOrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService) =>
            _tokenService = tokenService;

        // POST: api/auth/login
        // NOTE: This is a minimal example. Replace with real user validation.
        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("username and password are required");

            // Minimal demo validation: accept "admin" / "password"
            if (request.Username != "admin" || request.Password != "password")
                return Unauthorized();

            var roles = new List<string> { "User" };
            if (request.Username == "admin")
                roles.Add("Admin");

            var token = _tokenService.CreateToken(request.Username, roles);
            return Ok(new LoginResponse { Token = token });
        }
    }
}