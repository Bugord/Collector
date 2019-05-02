using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Models.Charge;
using Stripe;

namespace Collector.BL.Services.ChargeService
{
    public interface IStripeChargeService
    {
        Task<Charge> Charge(ChargeDTO model);
        Task GetAllCharges();
    }
}