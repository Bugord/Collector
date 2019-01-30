using System;
using System.Data;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using Collector.Attributes;
using Collector.BL.Exceptions;
using Collector.BL.Models.Debt;
using Collector.BL.Models.Feedback;
using Collector.BL.Services.DebtsService;
using Collector.BL.Services.Feedback;
using Collector.DAO.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Collector.Controllers
{
    [ApiController]
    [Route("api/feedback")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackServiceService)
        {
            _feedbackService = feedbackServiceService;
        }

        [HttpGet("getFeedbacks")]
        [Authorize]
        public async Task<IActionResult> GetFeedbacks()
        {
            try
            {
                var data = await _feedbackService.GetFeedbacksAsync(0,0);
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

        [HttpGet("getFeedback")]
        [Authorize]
        public async Task<IActionResult> GetFeedback(long id)
        {
            try
            {
                var data = await _feedbackService.GetFeedbackAsync(id);
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


        [HttpPost("addFeedback")]
        [Authorize]
        public async Task<IActionResult> AddFeedback(FeedbackAddDTO model)
        {
            try
            {
                var data = await _feedbackService.AddFeedbackAsync(model);
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

        [HttpPost("addFeedbackMessage")]
        [Authorize]
        public async Task<IActionResult> AddFeedbackMessage(FeedbackMessageAddDTO model)
        {
            try
            {
                var data = await _feedbackService.AddFeedbackMessageAsync(model);
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

        [HttpGet("getMessages")]
        [Authorize]
        public async Task<IActionResult> GetFeedbackMessages(long id)
        {
            try
            {
                var data = await _feedbackService.GetFeedbackMessagesAsync(id);
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

        [HttpPut("close/{id}")]
        public async Task<IActionResult> CloseFeedback(long id)
        {
            try
            {
                var data = await _feedbackService.CloseFeedbackAsync(id);
                return Ok(data);
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
            catch (NoPermissionException)
            {
                return Forbid();
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }

    }
}
