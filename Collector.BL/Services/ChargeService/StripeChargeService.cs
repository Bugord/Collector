using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Models.Charge;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Http;
using Stripe;

namespace Collector.BL.Services.ChargeService
{
    public class StripeChargeService : IStripeChargeService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<User> _userRepository;

        public StripeChargeService(IHttpContextAccessor httpContextAccessor, IRepository<User> userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public async Task<Charge> Charge(ChargeDTO charge)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();
            
            var ownerUser = await _userRepository.GetByIdAsync(ownerId);

            var customers = new CustomerService();
            var charges = new Stripe.ChargeService();

            var customer = await customers.CreateAsync(new CustomerCreateOptions
            {
                Email = ownerUser.Email,
                SourceToken = charge.Token
            });

            var newCharge = await charges.CreateAsync(new ChargeCreateOptions
            {
                Amount = charge.Value,
                Description = "Test charge",
                Currency = charge.Currency,
                CustomerId = customer.Id
            });

            return newCharge;
        }

        public async Task GetAllCharges()
        {
            var service = new Stripe.ChargeService();
            var options = new ChargeListOptions
            {
                Limit = 3,
            };
            var charges = await service.ListAsync(options);
        }
    }
}