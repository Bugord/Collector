using System;
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

        [HttpGet("getFeedback")]
        [Authorize]
        public async Task<IActionResult> GetFeedback(long id)
        {
            try
            {
                var data = await _feedbackService.GetFeedback(id);
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
                return BadRequest(new {e.Message});
            }
        }

        [HttpPost("addFeedbackMessage")]
        [Authorize]
        public async Task<IActionResult> AddFeedbackMessage(FeedbackMessageAddDTO model)
        {
            try
            {
                await _feedbackService.AddFeedbackMessageAsync(model);
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }

        [HttpPut("close")]
        [AuthorizeRoles(Role.Admin, Role.Moderator)]
        public async Task<IActionResult> CloseFeedback(long id)
        {
            try
            {
                await _feedbackService.CloseFeedback(id);
                return Ok();
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
