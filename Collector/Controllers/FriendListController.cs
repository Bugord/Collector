using System;
using System.Threading.Tasks;
using Collector.BL.Exceptions;
using Collector.BL.Services.AuthorizationService;
using Collector.BL.Models.FriendList;
using Collector.BL.Services.FriendListService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Collector.Controllers
{
    [ApiController]
    [Route("api")]
    public class FriendListController : ControllerBase
    {
        private readonly IFriendListService _friendService;

        public FriendListController(ITokenService tokenService, IFriendListService friendService)
        {
            _friendService = friendService;
        }

        [HttpPost("addFriend")]
        [Authorize]
        public async Task<IActionResult> Addfriend(FriendAddDTO model)
        {
            try
            {
                var data = await _friendService.AddFriendAsync(model);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }

        [HttpPut("updateFriend/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateFriend(long id, FriendUpdateDTO model)
        {
            try
            {
                await _friendService.UpdateFriendAsync(model);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }

        [HttpDelete("removeFriend/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveFriend(long id)
        {
            try
            {
                await _friendService.RemoveFriendAsync(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }
   

        [HttpGet("getAllFriends")]
        [Authorize]
        public async Task<IActionResult> GetAllFriends()
        {
            try
            {
                return Ok(new
                {
                    friends = await _friendService.GetAllFriendsAsync(),
                    invites = await _friendService.GetAllInvitesAsync()
                });
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }

        [HttpPost("inviteFriend")]
        [Authorize]
        public async Task<IActionResult> InviteFriend(FriendInviteDTO model)
        {
            try
            {
                await _friendService.InviteFriendAsync(model);
                return Ok("Friend invite was sended");
            }
            catch (NoPermissionException e)
            {
                return BadRequest(new { e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }

        [HttpPost("acceptFriend")]
        [Authorize]
        public async Task<IActionResult> ApproveFriend(FriendAcceptDTO model)
        {
            try
            {
                await _friendService.ApproveInviteAsync(model);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }
    }
}
