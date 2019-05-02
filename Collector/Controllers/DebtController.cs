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

        [HttpGet("getPayments")]
        [Authorize]
        public async Task<IActionResult> GetPayments()
        {
            try
            {
                var data = await _debtService.GetPaymentsAsync();
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

        [HttpGet("payNotifications")]
        [Authorize]
        public async Task<IActionResult> GetPayNotifications()
        {
            try
            {
                var data = await _debtService.GetPayNotificationsAsync();
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

        [HttpGet("getCurrencies")]
        [Authorize]
        public async Task<IActionResult> GetCurrencies()
        {
            try
            {
                var data = await _debtService.GetCurrenciesAsync();
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

        [HttpPut("payNotifications/accept/{id}")]
        [Authorize]
        public async Task<IActionResult> AcceptPayNotification(long id)
        {
            try
            {
                await _debtService.AcceptPayNotificationAsync(id);
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

        [HttpPut("payNotifications/deny/{id}")]
        [Authorize]
        public async Task<IActionResult> DenyPayNotification(long id)
        {
            try
            {
                await _debtService.DenyPayNotificationAsync(id);
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

        [HttpPost("addDebt")]
        [Authorize]
        public async Task<IActionResult> AddDebt(DebtAddDTO model)
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
                return BadRequest(new {e.Message});
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
                return BadRequest(new {e.Message});
            }
        }

        [HttpPut("debt/pay")]
        [Authorize]
        public async Task<IActionResult> PayDebt(DebtPayDTO model)
        {
            try
            {
                await _debtService.DebtPayAsync(model);
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
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }

        [HttpGet("debt/{id}/changes")]
        [Authorize]
        public async Task<IActionResult> GetDebtChangesById([FromQuery] ChangeSearchDTO model)
        {
            try
            {
                var data = await _debtService.GetDebtChangesByIdAsync(model);
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

        [HttpPost("getAllDebts")]
        [Authorize]
        public async Task<IActionResult> GetAllDebts(DebtSearchObjectDTO model)
        {
            try
            {
                var data = await _debtService.GetAllDebtsAsync(model);
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
                return BadRequest(new {e.Message});
            }
        }

        [HttpPut("updateDebt")]
        [Authorize]
        public async Task<IActionResult> UpdateDebt(DebtUpdateDTO model)
        {
            try
            {
                var data = await _debtService.UpdateDebtAsync(model);
                return Ok(new {debt = data});
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
            catch (Exception e)
            {
                return BadRequest(new {e.Message});
            }
        }
    }
}