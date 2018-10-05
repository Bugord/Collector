using System;
using System.Threading.Tasks;
using Collector.BL.Models.Debt;
using Collector.BL.Services.DebtsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }

        [HttpPut("updateDebt/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateDebt(long id, DebtUpdateDTO model)
        {
            try
            {
                await _debtService.UpdateDebtAsync(model);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message });
            }
        }
    }
}
