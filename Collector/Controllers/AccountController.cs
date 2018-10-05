using System;
using System.Threading.Tasks;
using Collector.BL.Models.Authorization;
using Collector.BL.Services.AuthorizationService;
using Microsoft.AspNetCore.Mvc;
using Collector.BL.Services.FriendListService;
using Collector.BL.Services.UserService;
using Microsoft.AspNetCore.Authorization;

namespace Collector.Controllers
{
    [ApiController]
    [Route("api")]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AccountController(ITokenService tokenService, IFriendListService friendService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPut("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            try
            {
                await _userService.ChangePasswordAsync(model);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }

        [AllowAnonymous]
        [HttpPut("resetPassword/{email}")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            try
            {
                await _userService.ResetPasswordAsync(email);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }

        [AllowAnonymous]
        [HttpPut("resetPassword")]
        public async Task<IActionResult> ResetPasswordToken(ResetPasswordDTO model)
        {
            try
            {
                await _userService.ResetPasswordTokenAsync(model);
                return Ok();
            }
            catch (FormatException)
            {
                return BadRequest(new {Message = "Token is not valid"});
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }


        [HttpPut("changeProfile")]
        [Authorize]
        public async Task<IActionResult> ChangeProfile(ChangeProfileDTO model)
        {
            try
            {
                var data = await _userService.ChangeProfileAsync(model);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            try
            {
                var data = await _tokenService.RegisterAsync(model);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            try
            {
                var data = await _tokenService.LoginAsync(model);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }
    }
}