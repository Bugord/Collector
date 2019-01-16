using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Collector.BL.Exceptions;
using Collector.BL.Extentions;
using Collector.BL.Models.Debt;
using Collector.BL.SignalR;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<MainHub> _hubContext;

        public DebtService(IRepository<Debt> debtRepository, IRepository<Friend> friendRepository,
            IHttpContextAccessor httpContextAccessor, IRepository<User> userRepository,
            IRepository<Change> changeRepository, IHubContext<MainHub> hubContext)
        {
            _debtRepository = debtRepository;
            _friendRepository = friendRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _changeRepository = changeRepository;
            _hubContext = hubContext;
        }


        public async Task<DebtReturnDTO> AddDebtAsync(DebtAddDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var owner = await _userRepository.GetByIdAsync(ownerId);
            
            var friend = await _friendRepository.GetByIdAsync(model.FriendId, friend1 => friend1.FriendUser);
            if(friend == null)
                throw new ArgumentException("Friend with this id does not exist");

            var newDebt = model.DebtAddDTOToDebt(friend, owner);
            var addedDebt = await _debtRepository.InsertAsync(newDebt);

            if (friend.FriendUser != null && addedDebt.Synchronize)
                await _hubContext.Clients.User(friend.FriendUser.Id.ToString()).SendAsync("UpdateDebts");

            return addedDebt.DebtToReturnDebtDTO();
        }

        public async Task<DebtReturnDTO> GetDebtByIdAsync(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

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

        public async Task<ChangesReturnDTO> GetDebtChangesByIdAsync(ChangeSearchDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var changes = (await _changeRepository.GetAllAsync(change =>
                    change.ChangedDebt.Id == model.Id, change => change.FieldChanges))
                .OrderByDescending(change => change.Created);

            var changesCount = changes.Count();

            var changesTaken = await changes.Skip(model.Offset).Take(model.Take)
                .Select(change => change.ToChangeReturnDTO()).ToListAsync();

            return new ChangesReturnDTO {Changes = changesTaken, HasMore = model.Offset + model.Take < changesCount};
        }


        public async Task<IList<DebtReturnDTO>> GetAllDebtsAsync(DebtSearchObjectDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var ownerUser = await (await _userRepository.GetAllAsync(user => user.Id == ownerId, user => user.Friends))
                .Include("Friends.FriendUser").FirstOrDefaultAsync();

            var userFriends = ownerUser.Friends;

            var debtsToReturn =
                await (await _debtRepository.GetAllAsync(debt =>
                        debt.Owner.Id == ownerId || debt.Friend.FriendUser.Id == ownerId && debt.Synchronize))
                    .Include(debt => debt.Friend).Where(model.GetExpression(ownerId))
                    .Skip(model.Offset)
                    .Take(model.Take)
                    .Select(debt => debt.DebtToReturnDebtDTO(
                        debt.Owner.Id == ownerId,
                        userFriends.FirstOrDefault(friend => friend.FriendUser.Id == debt.Owner.Id)
                    ))
                    .ToListAsync();

            return debtsToReturn;
        }

        public async Task RemoveDebtAsync(long debtId)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

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
            {
                await _debtRepository.RemoveByIdAsync(debtId);
                if (debt.Friend.FriendUser != null)
                    await _hubContext.Clients.User(debt.Friend.FriendUser.Id.ToString()).SendAsync("UpdateDebts");
            }
        }

        public async Task<DebtReturnDTO> UpdateDebtAsync(DebtUpdateDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var oldDebt = await _debtRepository.GetByIdAsync(model.DebtId, debt => debt.Owner, debt => debt.Friend);
            if (oldDebt.Owner.Id != ownerId)
                throw new NoPermissionException("User is not owner of this debt");

            var friend = await _friendRepository.GetByIdAsync(model.FriendId, friend1 => friend1.FriendUser);

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
            var updatedDebt = await _debtRepository.UpdateAsync(oldDebt);
            if (friend.FriendUser != null)
                await _hubContext.Clients.User(friend.FriendUser.Id.ToString()).SendAsync("UpdateDebts");

            return updatedDebt.DebtToReturnDebtDTO();
        }
    }
}