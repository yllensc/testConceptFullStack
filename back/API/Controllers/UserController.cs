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
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync(RegisterDto model)
        {
            var result = await _userService.RegisterAsync(model);
            return Ok(result);
        }
        [HttpPost("token")]
        public async Task<ActionResult> GetTokenAsync(LogDto model)
        {
            var result = await _userService.GetTokenAsync(model);
            return Ok(result);
        }
        [HttpPost("addrol")]
        public async Task<ActionResult> AddRoleAsync(AddRoleDto model)
        {
            var result = await _userService.AddRoleAsync(model);
            return Ok(result);
        }
        [HttpPost("getRefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _userService.RefreshTokenAsync(refreshToken);
            if (!string.IsNullOrEmpty(response.RefreshToken))
                SetRefreshTokenInCookie(response.RefreshToken);
            return Ok(response);
        }


        private void SetRefreshTokenInCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(10),
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

    }


}