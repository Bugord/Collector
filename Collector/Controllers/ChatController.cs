using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collector.BL.Models.Chat;
using Collector.BL.Services.ChatService;
using Collector.BL.Services.UploadService;
using Collector.DAO.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Collector.Controllers
{
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUploadService _uploadService;

        public ChatController(IChatService chatService, IUploadService uploadService)
        {
            _chatService = chatService;
            _uploadService = uploadService;
        }

        [HttpGet("chat/getMessages")]
        [Authorize]
        public async Task<IActionResult> GetDebtById(ChatMessagesGetDTO model)
        {
            try
            {
                var data = await _chatService.GetAllChatMessagesAsync(model);
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

        [HttpPost("chat/upload")]
        [Authorize]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                var data = await _uploadService.UploadFileAsync(file, UploadType.Chat);
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
