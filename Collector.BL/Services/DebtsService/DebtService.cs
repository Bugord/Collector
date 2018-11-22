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
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Change> _changeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DebtService(IRepository<Debt> debtRepository, IRepository<Friend> friendRepository,
            IHttpContextAccessor httpContextAccessor, IRepository<User> userRepository,
            IRepository<Change> changeRepository)
        {
            _debtRepository = debtRepository;
            _friendRepository = friendRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _changeRepository = changeRepository;
        }


        public async Task<DebtReturnDTO> AddDebtAsync(DebtAddDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var owner = await _userRepository.GetByIdAsync(ownerId);
            var friend = await _friendRepository.GetByIdAsync(model.FriendId);

            var newDebt = model.DebtAddDTOToDebt(friend, owner);

            return (await _debtRepository.InsertAsync(newDebt)).DebtToReturnDebtDTO();
        }

        public async Task<DebtReturnDTO> GetDebtByIdAsync(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var ownerUser = await _userRepository.GetByIdAsync(ownerId, user => user.Friends);

            var debtToReturn =
                await _debtRepository.GetByIdAsync(id, debt => debt.Friend.FriendUser, debt => debt.Owner);

            if (debtToReturn == null)
                throw new SqlNullValueException("Debt with this id is not exist");

            var isOwner = debtToReturn.Owner.Id == ownerId;
            var isUser = debtToReturn.Friend.FriendUser != null && debtToReturn.Friend.FriendUser.Id == ownerId;

            if (!(isOwner || isUser))
                throw new NoPermissionException("User is not owner of this debt");

            return debtToReturn.DebtToReturnDebtDTO(debtToReturn.Owner.Id == ownerId,
                ownerUser.Friends.FirstOrDefault(friend => friend.FriendUser == debtToReturn.Owner));
        }

        public async Task<IList<ChangeReturnDTO>> GetDebtChangesByIdAsync(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var changes = (await _changeRepository.GetAllAsync(change => change.ChangedDebt.Id == id))
                .Include("FieldChanges")
                .Include("ChangedBy");


            return await changes.Select(change => change.ToChangeReturnDTO()).ToListAsync();
        }


        public async Task<IList<DebtReturnDTO>> GetAllDebtsAsync()
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var ownerUser = await (await _userRepository.GetAllAsync(user => user.Id == ownerId, user => user.Friends))
                .Include("Friends.FriendUser").FirstOrDefaultAsync();
          

            var debtsToReturn =
                await (await _debtRepository.GetAllAsync(debt =>
                            debt.Owner.Id == ownerId || (debt.Friend.FriendUser.Id == ownerId && debt.Synchronize),
                        debt => debt.Friend.FriendUser))
                    .Select(debt => debt.DebtToReturnDebtDTO(debt.Owner.Id == ownerId,
                        ownerUser.Friends.FirstOrDefault(friend => friend.FriendUser == debt.Owner)))
                    .ToListAsync();

            return debtsToReturn;
        }

        public async Task RemoveDebtAsync(long debtId)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var debt = await _debtRepository.GetByIdAsync(debtId, debt1 => debt1.Friend.FriendUser,
                debt1 => debt1.Owner);

            if (debt == null)
                throw new SqlNullValueException("Debt with this id is not exist");

            var isOwner = debt.Owner.Id == ownerId;
            var isUser = debt.Friend.FriendUser != null && debt.Friend.FriendUser.Id == ownerId;

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

            var oldDebt = await _debtRepository.GetByIdAsync(model.DebtId, debt => debt.Owner, debt => debt.Friend);
            if (oldDebt.Owner.Id != ownerId)
                throw new NoPermissionException("User is not owner of this debt");

            var friend = await _friendRepository.GetByIdAsync(model.FriendId);

            oldDebt.Update(model, friend, out var listOfFieldChanges);

            if (listOfFieldChanges.Count != 0)
            {
                var change = new Change
                {
                    ChangedBy = await _userRepository.GetByIdAsync(ownerId),
                    FieldChanges = listOfFieldChanges,
                    ChangedDebt = oldDebt
                };


                await _changeRepository.InsertAsync(change);
            }

            oldDebt.ModifiedBy = ownerId;
            await _debtRepository.UpdateAsync(oldDebt);
        }
    }
}