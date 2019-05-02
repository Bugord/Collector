using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Collector.BL.Exceptions;
using Collector.BL.Models.Authorization;
using Collector.BL.Services.AuthorizationService;
using Microsoft.AspNetCore.Mvc;
using Collector.BL.Services.FriendListService;
using Collector.BL.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new
                {
                    Message =
                        "The record you attempted to edit was modified by another user after you got the original value"
                });
            }
            catch (ArgumentException e)
            {
                return BadRequest(new {e.Message});
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
            catch (SqlNullValueException e)
            {
                return NotFound(new {e.Message});
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new
                {
                    Message =
                        "The record you attempted to edit was modified by another user after you got the original value"
                });
            }
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }

        [AllowAnonymous]
        [HttpPut("confirmEmail/{token}")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            try
            {
                await _userService.ConfirmEmailAsync(token);
                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest(new {Message = "Token is not valid"});
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Exception while email confirmation" });
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
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new
                {
                    Message =
                        "The record you attempted to edit was modified by another user after you got the original value"
                });
            }
            catch (SqlNullValueException e)
            {
                return NotFound(new { e.Message });
            }
            catch (SecurityTokenInvalidLifetimeException e)
            {
                return BadRequest(new {e.Message});
            }
            catch (FormatException)
            {
                return BadRequest(new {Message = "Token is not valid"});
            }
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }

        [HttpPut("changeProfile")]
        [Authorize]
        public async Task<IActionResult> ChangeProfile([FromForm]ChangeProfileDTO model)
        {
            try
            {
                var data = await _userService.ChangeProfileAsync(model);
                return Ok(data);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new
                {
                    Message =
                        "The record you attempted to edit was modified by another user after you got the original value"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (AlreadyExistsException e)
            {
                return BadRequest(new { e.Message });
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
            catch (AlreadyExistsException e)
            {
                return BadRequest(new {e.Message});
            }
            catch (ServerFailException e)
            {
                return BadRequest(new { e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo(long? id)
        {
            try
            {
                var data = await _userService.GetUserInfoAsync(id);
                return Ok(data);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
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
            catch (AuthenticationFailException e)
            {
                return BadRequest(new { e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }
    }
}