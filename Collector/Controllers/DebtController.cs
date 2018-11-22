using System;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using Collector.BL.Exceptions;
using Collector.BL.Models.Debt;
using Collector.BL.Services.DebtsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Collector.Controllers
{
    [ApiController]
    [Route("api")]
    public class DebtController : ControllerBase
    {
        private readonly IDebtService _debtService;

        public DebtController(IDebtService debtService)
        {
            _debtService = debtService;
        }

        [HttpPost("addDebt")]
        [Authorize]
        public async Task<IActionResult> AddFriend(DebtAddDTO model)
        {
            try
            {
                var data = await _debtService.AddDebtAsync(model);
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

        [HttpGet("debt/{id}")]
        [Authorize]
        public async Task<IActionResult> GetDebtById(long id)
        {
            try
            {
                var data = await _debtService.GetDebtByIdAsync(id);
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

        [HttpGet("debt/{id}/changes")]
        [Authorize]
        public async Task<IActionResult> GetDebtChangesById(long id)
        {
            try
            {
                var data = await _debtService.GetDebtChangesByIdAsync(id);
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

        [HttpGet("getAllDebts")]
        [Authorize]
        public async Task<IActionResult> GetAllDebts()
        {
            try
            {
                var data = await _debtService.GetAllDebtsAsync();
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

        [HttpDelete("removeDebt/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveDebt(long id)
        {
            try
            {
                await _debtService.RemoveDebtAsync(id);
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
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (NoPermissionException e)
            {
                return BadRequest(new {e.Message});
            }
            catch (SqlNullValueException e)
            {
                return NotFound(new {e.Message});
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }

        [HttpPut("updateDebt")]
        [Authorize]
        public async Task<IActionResult> UpdateDebt(DebtUpdateDTO model)
        {
            try
            {
                await _debtService.UpdateDebtAsync(model);
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { Message = "The record you attempted to edit was modified by another user after you got the original value" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
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
    }
}
