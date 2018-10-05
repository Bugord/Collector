using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Collector.BL.Exceptions;
using Collector.BL.Extentions;
using Collector.BL.Models.Debt;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Collector.BL.Services.DebtsService
{
    public class DebtService : IDebtService
    {
        private readonly IRepository<Debt> _debtRepository;
        private readonly IRepository<Friend> _friendRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DebtService(IRepository<Debt> debtRepository, IRepository<Friend> friendRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _debtRepository = debtRepository;
            _friendRepository = friendRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<DebtReturnDTO> AddDebtAsync(DebtAddDTO model)
        {
            //var ownerId = long.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var idClaim =_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null) 
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);
                

            var newDebt = model.DebtAddDTOToDebt(ownerId);

            return (await _debtRepository.InsertAsync(newDebt)).DebtToReturnDebtDTO();
        }

        public async Task<IList<DebtReturnDTO>> GetAllDebtsAsync()
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var friends =
                await (await _friendRepository.GetAllAsync(friend => friend.UserId == ownerId || friend.OwnerId == ownerId)
                    ).Select(o => o.Id).ToListAsync();

            var debtsToReturn = await (await _debtRepository.GetAllAsync(debt => friends.Contains(debt.FriendId))).Select(debt => debt.DebtToReturnDebtDTO(debt.OwnerId == ownerId)).ToListAsync();
            
            return debtsToReturn;
        }

        public async Task RemoveDebtAsync(long debtId)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var debt = await _debtRepository.GetByIdAsync(debtId);
            if (debt == null)
                throw new SqlNullValueException("Debt with this id is not exist");


            var friend = await _friendRepository.GetByIdAsync(debt.FriendId);

            var isOwner = debt.OwnerId == ownerId;
            var isUser = friend.UserId == ownerId || friend.OwnerId == ownerId;

            if (!(isOwner || isUser))
                throw new NoPermissionException("User is not owner of this debt");

            if (!isOwner)
            {
                debt.Synchronize = false;
                await _debtRepository.UpdateAsync(debt);
            }
            else
                await _debtRepository.RemoveByIdAsync(debtId);
        }

        public async Task UpdateDebtAsync(DebtUpdateDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var oldDebt = await _debtRepository.GetByIdAsync(model.DebtId);

            if (oldDebt.OwnerId != ownerId)
                throw new NoPermissionException("User is not owner of this debt");

            oldDebt.Update(model);
            oldDebt.ModifiedBy = ownerId;
            await _debtRepository.UpdateAsync(oldDebt);
        }
    }
}