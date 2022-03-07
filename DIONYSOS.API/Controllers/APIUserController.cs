using DIONYSOS.API.Authentification;
using DIONYSOS.API.Data.ModelsApplicatif;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace DIONYSOS.API.Controllers
{
    [ApiController]
    [Route("api/apiuser")]
    public class APIUserController : ControllerBase
    {
        private readonly IJwtAuthenticationService _jwtAuthenticationService;
        private readonly IConfiguration _config;

        public APIUserController(IJwtAuthenticationService JwtAuthenticationService, IConfiguration config)
        {
            _jwtAuthenticationService = JwtAuthenticationService;
            _config = config;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "L'authentification a réussit")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(EmptyResult), Description = "L'authentification a échoué")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _jwtAuthenticationService.Authenticate(model.Email);
            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password ,user.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                };
                var token = _jwtAuthenticationService.GenerateToken(_config["Jwt:Key"], claims);
                return Ok(token);
            }
            return Unauthorized();
        }
    }
}
