using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collector.BL.Models.Chat;
using Collector.BL.Services.ChatService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Collector.Controllers
{
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("chat/getMessages")]
        [Authorize]
        public async Task<IActionResult> GetDebtById(ChatGetMessagesDTO model)
        {
            try
            {
                var data = await _chatService.GetAllChatMessages(model);
                return Ok(data);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }
    }
}
