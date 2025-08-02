using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("/api/authenticate")]
    public class AuthenticateController : ControllerBase
    {

        private readonly JwtTokenService _jwtTokenService;
        public AuthenticateController(JwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
           var response = await _jwtTokenService.Login(request);
           return Ok(response);
          
        }


       [HttpPost("register")]
       public async Task<IActionResult> Register([FromBody] RegisterRequest request)
       {
           var response = await _jwtTokenService.Register(request);
           return Created("/api/authenticate/login", response);

       }
        
    }

}
