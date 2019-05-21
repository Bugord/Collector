using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collector.BL.Models.Charge;
using Collector.BL.Services.ChargeService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Collector.Controllers
{
    [ApiController]
    [Route("Api")]
    public class ChargeController : ControllerBase
    {
        private IStripeChargeService _chargeService;

        public ChargeController(IStripeChargeService chargeService)
        {
            _chargeService = chargeService;
        }

        [HttpPost("charge")]
        [Authorize]
        public async Task<IActionResult> Charge(ChargeDTO model)
        {
            try
            {
                await _chargeService.Charge(model);
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
        }}
}
