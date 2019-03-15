using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly IRepository<PayNotification> _payNotificationRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<Currency> _currenciesRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<MainHub> _hubContext;

        public DebtService(IRepository<Debt> debtRepository, IRepository<Friend> friendRepository,
            IHttpContextAccessor httpContextAccessor, IRepository<User> userRepository,
            IRepository<Change> changeRepository, IHubContext<MainHub> hubContext,
            IRepository<PayNotification> payNotificationRepository, IRepository<Notification> notificationRepository,
            IRepository<Currency> currenciesRepository)
        {
            _debtRepository = debtRepository;
            _friendRepository = friendRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _changeRepository = changeRepository;
            _hubContext = hubContext;
            _payNotificationRepository = payNotificationRepository;
            _notificationRepository = notificationRepository;
            _currenciesRepository = currenciesRepository;
        }

        public async Task<IList<PaymentDTO>> GetPaymentsAsync()
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var payments = await (await _payNotificationRepository.GetAllAsync(notification =>
                    notification.Payer.Id == ownerId || notification.UserToPay.Id == ownerId))
                .Include(notification => notification.UserToPay)
                .Include(notification => notification.Payer)
                .Include(notification => notification.Debt)
                .ThenInclude(debt => debt.Currency)
                .Select(notification => notification.ToPaymentDTO(ownerId)).ToListAsync();

            return payments;
        }

        public async Task<IList<CurrencyReturnDTO>> GetCurrenciesAsync()
        {
            var currencies =
                (await _currenciesRepository.GetAllAsync()).Select(currency => currency.ToCurrencyReturnDTO());

            return await currencies.ToArrayAsync();
        }

        public async Task AcceptPayNotificationAsync(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var payNotification =
                await _payNotificationRepository.GetFirstAsync(notification => notification.Id == id &&
                                                                               notification.UserToPay.Id == ownerId,
                    notification => notification.Payer, notification => notification.UserToPay);

            if (payNotification == null)
                throw new ArgumentException(
                    "Pay notification with such id does not exist or you are not owner of this debt");

            payNotification.Confirmed = true;
            payNotification.Approved = true;
            await _payNotificationRepository.UpdateAsync(payNotification);

            var acceptNotification = new Notification
            {
                Recipient = payNotification.Payer,
                Message = $"{payNotification.UserToPay.Username} accepted your debt pay " +
                          $"({(payNotification.Debt.IsMoney ? payNotification.Value + "$" : payNotification.Debt.Name)}$)",
                Confirmed = false
            };

            await _notificationRepository.InsertAsync(acceptNotification);

            await _hubContext.Clients.User(acceptNotification.Recipient.Id.ToString()).SendAsync("UpdateNotifications");
        }

        public async Task<DebtReturnDTO> DenyPayNotificationAsync(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var payNotification =
                await _payNotificationRepository.GetFirstAsync(
                    notification => notification.Id == id && notification.UserToPay.Id == ownerId,
                    notification => notification.Debt, notification => notification.Payer,
                    notification => notification.UserToPay);

            if (payNotification == null)
                throw new ArgumentException(
                    "Pay notification with such id does not exist or you are not owner of this debt");

            if (payNotification.Confirmed)
                throw new ArgumentException("Pay notification was already close");

            var debtToPay =
                await _debtRepository.GetByIdAsync(payNotification.Debt.Id, debt => debt.Friend, debt => debt.Currency);

            debtToPay.CurrentValue -= payNotification.Value;
            debtToPay.IsClosed = false;

            await _debtRepository.UpdateAsync(debtToPay);

            payNotification.Confirmed = true;
            payNotification.Approved = false;
            await _payNotificationRepository.UpdateAsync(payNotification);

            await _hubContext.Clients.User(payNotification.Payer.Id.ToString())
                .SendAsync("UpdateDebtById", payNotification.Debt.Id);

            var denyNotification = new Notification
            {
                Recipient = payNotification.Payer,
                Message =
                    $"{payNotification.UserToPay.Username} denied your debt pay " +
                    $"({(payNotification.Debt.IsMoney ? payNotification.Value + "$" : payNotification.Debt.Name)})",
                Confirmed = false
            };

            await _notificationRepository.InsertAsync(denyNotification);

            await _hubContext.Clients.User(denyNotification.Recipient.Id.ToString()).SendAsync("UpdateNotifications");

            return debtToPay.DebtToReturnDebtDTO();
        }

        public async Task<IList<PayNotificationReturnDTO>> GetPayNotificationsAsync()
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var notifications =
                (await _payNotificationRepository.GetAllAsync(notification =>
                    notification.UserToPay.Id == ownerId && !notification.Confirmed && notification.Approved))
                .Include(notification => notification.Payer)
                .Include(notification => notification.Debt)
                .ThenInclude(debt => debt.Friend);


            return await notifications.Select(notification => notification.ToPayNotificationReturnDTO()).ToListAsync();
        }

        public async Task<DebtReturnDTO> AddDebtAsync(DebtAddDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var owner = await _userRepository.GetByIdAsync(ownerId);

            var friend = await _friendRepository.GetByIdAsync(model.FriendId, friend1 => friend1.FriendUser);
            if (friend == null)
                throw new ArgumentException("Friend with this id does not exist");

            Currency currency = null;

            if (model.IsMoney)
            {
                if (model.Value == null)
                    throw new ArgumentException("Value can't be null");

                if (model.CurrencyId == null)
                    throw new ArgumentException("Currency id can't be null");

                currency = await _currenciesRepository.GetByIdAsync(model.CurrencyId.Value);
            }
            else if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Thing name can't be empty");


            var newDebt = model.DebtAddDTOToDebt(friend, owner, currency);
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
                await _debtRepository.GetByIdAsync(id, debt => debt.Friend.FriendUser, debt => debt.Owner,
                    debt => debt.Currency);

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

        public async Task<DebtReturnDTO> DebtPayAsync(DebtPayDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var ownerUser = await _userRepository.GetByIdAsync(ownerId, user => user.Friends);

            var debtToPay =
                await (await _debtRepository.GetAllAsync(debt => debt.Id == model.DebtId))
                    .Include(debt => debt.Currency)
                    .Include(debt => debt.Owner)
                    .Include(debt => debt.Friend)
                    .ThenInclude(friend => friend.FriendUser)
                    .FirstOrDefaultAsync();

            if (debtToPay == null)
                throw new ArgumentException("Debt is not exist");

            //if (!debtToPay.IsMoney)
            //    throw new ArgumentException("This debt type is not payable");
            if (debtToPay.IsMoney)
            {
                if (!debtToPay.Value.HasValue || !debtToPay.CurrentValue.HasValue)
                    throw new NoNullAllowedException("Value and CurrentValue can't be null");

                if (debtToPay.Value - debtToPay.CurrentValue < model.Value)
                    throw new ArgumentException("You can't pay more, than debt ");

                debtToPay.CurrentValue += model.Value;

                if (debtToPay.CurrentValue == debtToPay.Value)
                    debtToPay.IsClosed = true;
            }
            else
                debtToPay.IsClosed = true;


            if (debtToPay.Synchronize && (debtToPay.IsOwnerDebter && debtToPay.Owner.Id == ownerId ||
                                          !debtToPay.IsOwnerDebter && debtToPay.Friend.FriendUser.Id == ownerId))
            {
                var payNotification = new PayNotification
                {
                    CreatedBy = ownerId,
                    Confirmed = false,
                    Payer = ownerUser,
                    Value = debtToPay.IsMoney ? model.Value : null,
                    Message = model.Message,
                    Debt = debtToPay,
                    UserToPay = debtToPay.IsOwnerDebter ? debtToPay.Friend.FriendUser : debtToPay.Owner
                };

                await _payNotificationRepository.InsertAsync(payNotification);

                await _hubContext.Clients.User(payNotification.UserToPay.Id.ToString())
                    .SendAsync("UpdatePayNotifications");
                await _hubContext.Clients.User(payNotification.UserToPay.Id.ToString())
                    .SendAsync("UpdateDebtById", payNotification.Debt.Id);
            }

            await _debtRepository.UpdateAsync(debtToPay);

            return debtToPay.DebtToReturnDebtDTO(debtToPay.Owner.Id == ownerId,
                ownerUser.Friends.FirstOrDefault(friend => friend.FriendUser == debtToPay.Owner));
        }


        public async Task<IList<DebtReturnDTO>> GetAllDebtsAsync(DebtSearchObjectDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var ownerUser = await (await _userRepository.GetAllAsync(user => user.Id == ownerId))
                .Include(user => user.Friends)
                .ThenInclude(friend => friend.FriendUser)
                .Select(user => new {user.Friends})
                .FirstOrDefaultAsync();

            var userFriends = ownerUser.Friends;

            var debtsToReturn =
                await (await _debtRepository.GetAllAsync(debt =>
                            debt.Owner.Id == ownerId || debt.Friend.FriendUser.Id == ownerId && debt.Synchronize,
                        debt => debt.Currency))
                    .Include(debt => debt.Friend).Where(model.GetExpression(ownerId))
                    .OrderByDescending(debt => debt.Created)
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

            var oldDebt = await _debtRepository.GetByIdAsync(model.DebtId, debt => debt.Owner, debt => debt.Friend,
                debt => debt.Currency);
            if (oldDebt.Owner.Id != ownerId)
                throw new NoPermissionException("User is not owner of this debt");

            var friend = await _friendRepository.GetByIdAsync(model.FriendId, friend1 => friend1.FriendUser);

            Currency currency = null;

            if (model.IsMoney)
            {
                if (model.Value == null)
                    throw new ArgumentException("Value can't be null");

                if (model.CurrencyId == null)
                    throw new ArgumentException("Currency id can't be null");

                currency = await _currenciesRepository.GetByIdAsync(model.CurrencyId.Value);
            }
            else if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Thing name can't be empty");


            oldDebt.Update(model, friend, currency, out var listOfFieldChanges);

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