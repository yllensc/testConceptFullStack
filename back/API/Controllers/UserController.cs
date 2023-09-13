using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UserController : ApiBaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService){
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync(RegisterDto model){
            var result = await _userService.RegisterAsync(model);
            return Ok(result);
        }
        [HttpPost("token")]
        public async Task<ActionResult> GetTokenAsync(LogDto model){
            var result = await _userService.GetTokenAsync(model);
            return Ok(result);
        }
        [HttpPost("addrol")]
        public async Task<ActionResult> AddRoleAsync(AddRoleDto model){
            var result = await _userService.AddRoleAsync(model);
            return Ok(result);
        }
        [HttpPost]
        [Route("getRefreshToken")]
        public async Task<IActionResult> ObtenerRefreshToken([FromBody] RefreshTokenDto request) {

            var tokenHandler = new JwtSecurityTokenHandler();
            var isTokenExpired = tokenHandler.ReadJwtToken(request.TokenExpired);

            if (isTokenExpired.ValidTo > DateTime.UtcNow)
                return BadRequest(new AuthTokenResponseDto { Result = false, Msg = "El Token no ha expirado" });

            string idUsuario = isTokenExpired.Claims.First(x =>
                x.Type == "uid").Value.ToString();
            var authTokenResponse = await _userService.GetRefreshToken(request, int.Parse(idUsuario));
            if (authTokenResponse.Result)
                return Ok(authTokenResponse);
            else
                return BadRequest(authTokenResponse);
        
        }

    }

    
}