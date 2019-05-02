using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Collector.BL.Exceptions;
using Collector.BL.Extentions;
using Collector.BL.Models.Charge;
using Collector.BL.Models.Debt;
using Collector.BL.Services.ChargeService;
using Collector.BL.Services.ExchangeRateService;
using Collector.BL.SignalR;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Collector.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stripe;

namespace Collector.BL.Services.DebtsService
{
    public class DebtService : IDebtService
    {
        private readonly IRepository<Debt> _debtRepository;
        private readonly IRepository<Friend> _friendRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Change> _changeRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<Currency> _currenciesRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<MainHub> _hubContext;
        private readonly IStripeChargeService _stripeChargeService;
        private readonly IExchangeRateService _exchangeRateService;

        public DebtService(IRepository<Debt> debtRepository, IRepository<Friend> friendRepository,
            IHttpContextAccessor httpContextAccessor, IRepository<User> userRepository,
            IRepository<Change> changeRepository, IHubContext<MainHub> hubContext,
            IRepository<PayNotification> payNotificationRepository, IRepository<Notification> notificationRepository,
            IRepository<Currency> currenciesRepository, IRepository<Payment> paymentRepository,
            IStripeChargeService stripeChargeService, IHttpClientFactory clientFactory,
            IExchangeRateService exchangeRateService)
        {
            _debtRepository = debtRepository;
            _friendRepository = friendRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _changeRepository = changeRepository;
            _hubContext = hubContext;
            _notificationRepository = notificationRepository;
            _currenciesRepository = currenciesRepository;
            _paymentRepository = paymentRepository;
            _stripeChargeService = stripeChargeService;
            _exchangeRateService = exchangeRateService;
        }

        public async Task<IList<PaymentDTO>> GetPaymentsAsync()
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();


            var payments = await (await _paymentRepository.GetAllAsync(payment =>
                    payment.UserToPay.Id == ownerId || payment.Payer.Id == ownerId))
                .Include(payment => payment.Debt)
                .Include(payment => payment.Payer)
                .Include(payment => payment.UserToPay)
                .Include(payment => payment.Currency)
                .OrderByDescending(payment => payment.Created)
                .Select(payment => payment.ToPaymentDTO(ownerId)).ToListAsync();

            //= await (await _payNotificationRepository.GetAllAsync(notification =>
            //    notification.Payer.Id == ownerId || notification.UserToPay.Id == ownerId))
            //.OrderByDescending(notification => notification.Created)
            //.Include(notification => notification.UserToPay)
            //.Include(notification => notification.Payer)
            //.Include(notification => notification.Debt)
            //.ThenInclude(debt => debt.Currency)
            //.Select(notification => notification.ToPaymentDTO(ownerId)).ToListAsync();

            return payments;
        }

        public async Task<IList<CurrencyReturnDTO>> GetCurrenciesAsync()
        {
            var exchanges = await _exchangeRateService.GetExchangeRatesAsync();

            var currencies =
                (await _currenciesRepository.GetAllAsync()).Select(currency =>
                    currency.ToCurrencyReturnDTO(exchanges.CurrencyRates
                        .FirstOrDefault(rate => rate.Currency == currency).Rate));


            return await currencies.ToArrayAsync();
        }

        public async Task AcceptPayNotificationAsync(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();
            var paymentToAccept =
                await (await _paymentRepository.GetAllAsync(payment =>
                        payment.Id == id && payment.UserToPay.Id == ownerId))
                    .Include(payment => payment.Debt)
                       .ThenInclude(debt => debt.Currency)
                    .Include(payment => payment.Currency)
                    .Include(payment => payment.Payer)
                    .Include(payment => payment.UserToPay)
                    .FirstOrDefaultAsync();

            if (paymentToAccept == null)
            {
                throw new ArgumentException(
                    "Payment with such id does not exist or you are not owner of this debt");
            }

            if (paymentToAccept.PaymentType != PaymentType.Notification)
                throw new ArgumentException("You can't accept not notification payment");

            if (paymentToAccept.Debt.IsMoney)
            {
                if (paymentToAccept.Debt.CurrentValue + paymentToAccept.Value > paymentToAccept.Debt.Value)
                    throw new ArgumentException("Can't pay more, than debt");
            }

            paymentToAccept.Status = Status.Accepted;

            if (paymentToAccept.Debt.IsMoney)
            {
                paymentToAccept.Debt.CurrentValue += paymentToAccept.Value;
                paymentToAccept.Debt.PendingValue -= paymentToAccept.Value;
                if (Equals(paymentToAccept.Debt.CurrentValue, paymentToAccept.Debt.Value))
                {
                    paymentToAccept.Debt.IsClosed = true;
                }
            }
            else
            {
                paymentToAccept.Debt.IsClosed = true;
            }


            var acceptNotification = new Notification
            {
                Recipient = paymentToAccept.Payer,
                Message = $"{paymentToAccept.UserToPay.Username} accepted your debt pay " +
                          $"({(paymentToAccept.Debt.IsMoney ? paymentToAccept.Value + paymentToAccept.Debt.Currency.CurrencySymbol : paymentToAccept.Debt.Name)})",
                Confirmed = false
            };

            await _notificationRepository.InsertAsync(acceptNotification);

            await _hubContext.Clients.User(acceptNotification.Recipient.Id.ToString()).SendAsync("UpdateNotifications");
            await _hubContext.Clients.User(paymentToAccept.Payer.Id.ToString())
                .SendAsync("UpdateDebtById", paymentToAccept.Debt.Id);
            await _hubContext.Clients.User(paymentToAccept.UserToPay.Id.ToString())
                .SendAsync("UpdateDebtById", paymentToAccept.Debt.Id);
        }

        public async Task DenyPayNotificationAsync(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var paymentToDeny =
                await (await _paymentRepository.GetAllAsync(payment =>
                        payment.Id == id && payment.UserToPay.Id == ownerId))
                    .Include(payment => payment.Debt)
                        .ThenInclude(debt => debt.Currency)
                    .Include(payment => payment.Currency)
                    .Include(payment => payment.Payer)
                    .Include(payment => payment.UserToPay)
                    .FirstOrDefaultAsync();

            paymentToDeny.Status = Status.Denied;

            if (paymentToDeny.Debt.IsMoney)
            {
                paymentToDeny.Debt.PendingValue -= paymentToDeny.Value;
            }

            await _paymentRepository.UpdateAsync(paymentToDeny);
            await _debtRepository.UpdateAsync(paymentToDeny.Debt);

            var denyNotification = new Notification
            {
                Recipient = paymentToDeny.Payer,
                Message =
                    $"{paymentToDeny.UserToPay.Username} denied your debt pay " +
                    $"({(paymentToDeny.Debt.IsMoney ? paymentToDeny.Value + paymentToDeny.Debt.Currency.CurrencySymbol : paymentToDeny.Debt.Name)})",
                Confirmed = false
            };

            await _notificationRepository.InsertAsync(denyNotification);

            await _hubContext.Clients.User(denyNotification.Recipient.Id.ToString()).SendAsync("UpdateNotifications");
            await _hubContext.Clients.User(paymentToDeny.Payer.Id.ToString())
                .SendAsync("UpdateDebtById", paymentToDeny.Debt.Id);
            await _hubContext.Clients.User(paymentToDeny.UserToPay.Id.ToString())
                .SendAsync("UpdateDebtById", paymentToDeny.Debt.Id);
        }

        public async Task DebtPayAsync(DebtPayDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var debtToPay =
                await (await _debtRepository.GetAllAsync(debt => debt.Id == model.DebtId))
                    .Include(debt => debt.Owner)
                    .Include(debt => debt.Currency)
                    .Include(debt => debt.Friend)
                    .ThenInclude(friend => friend.FriendUser)
                    .FirstOrDefaultAsync();

            if (debtToPay == null)
                throw new ArgumentException("Debt is not exist");

            Currency payCurrency = null;
            decimal valueToPay = 0;
            Charge charge = null;

            if (debtToPay.IsMoney)
            {
                if (!debtToPay.Value.HasValue || !debtToPay.CurrentValue.HasValue)
                    throw new NoNullAllowedException("Value and CurrentValue can't be null");

                if (!model.CurrencyId.HasValue)
                    throw new ArgumentNullException(nameof(model.CurrencyId));

                if (!model.Value.HasValue)
                    throw new ArgumentNullException(nameof(model.Value));

                if (!model.IsNotification && string.IsNullOrWhiteSpace(model.Token))
                    throw new ArgumentException("Token can't be empty");


                payCurrency = await _currenciesRepository.GetByIdAsync(model.CurrencyId.Value);

                if (payCurrency == null)
                    throw new SqlNullValueException("Currency with such id not found");

                valueToPay = await _exchangeRateService.Convert(model.Value.Value, payCurrency, debtToPay.Currency);

                if (debtToPay.CurrentValue + debtToPay.PendingValue + valueToPay > debtToPay.Value)
                    throw new ArgumentException("You can't pay more, than debt ");

                debtToPay.PendingValue += valueToPay;

                if (!model.IsNotification)
                {
                    charge = await _stripeChargeService.Charge(new ChargeDTO
                    {
                        Value = (long) (valueToPay * 100),
                        Currency = debtToPay.Currency.CurrencySymbol,
                        Token = model.Token
                    });
                }
            }
            else
            {
                if (!model.IsNotification)
                    throw new ArgumentException("You can pay thing debts only by notification");
            }

            var userToPay = debtToPay.IsOwnerDebter ? debtToPay.Friend.FriendUser : debtToPay.Owner;
            var userPayer = debtToPay.IsOwnerDebter ? debtToPay.Owner : debtToPay.Friend.FriendUser;


            var newPayment = new Payment
            {
                CreatedBy = ownerId,
                Payer = userPayer,
                UserToPay = userToPay,
                Debt = debtToPay,
                Value = valueToPay,
                Currency = payCurrency,
                Message = model.Message,
                PaymentType = model.IsNotification ? PaymentType.Notification : PaymentType.Stripe,
                Status = Status.Pending,
                TransactionId = charge?.BalanceTransactionId,
                TransactionDate = charge?.Created
            };

            await _paymentRepository.InsertAsync(newPayment);
            await _debtRepository.UpdateAsync(debtToPay);

            if (newPayment.PaymentType == PaymentType.Notification && debtToPay.Synchronize &&
                (debtToPay.IsOwnerDebter && debtToPay.Owner.Id == ownerId ||
                 !debtToPay.IsOwnerDebter && debtToPay.Friend.FriendUser.Id == ownerId))
            {
                await _hubContext.Clients.User(userToPay.Id.ToString())
                    .SendAsync("UpdatePayNotifications");
            }


            await _hubContext.Clients.User(ownerId.ToString())
                .SendAsync("UpdateDebtById", debtToPay.Id);
        }


        public async Task<IList<PayNotificationReturnDTO>> GetPayNotificationsAsync()
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var payments =
                (await _paymentRepository.GetAllAsync(payment => payment.UserToPay.Id == ownerId
                                                                 && payment.Status == Status.Pending &&
                                                                 payment.PaymentType == PaymentType.Notification))
                .OrderByDescending(payment => payment.Created)
                .Include(payment => payment.Debt)
                .Include(payment => payment.Payer)
                .Include(payment => payment.Currency);

            //var notifications =
            //    (await _payNotificationRepository.GetAllAsync(notification =>
            //        notification.UserToPay.Id == ownerId && !notification.Confirmed))
            //    .OrderByDescending(notification => notification.Created)
            //    .Include(notification => notification.Payer)
            //    .Include(notification => notification.Debt)
            //    .Include(notification => notification.Debt.Friend)
            //    .Include(notification => notification.Debt.Currency);

            //return await notifications.Select(notification => notification.ToPayNotificationReturnDTO()).ToListAsync();
            return await payments.Select(payment => payment.ToNotificationReturnDTO()).ToListAsync();
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
            return new ChangesReturnDTO
            {
                Changes = changesTaken,
                HasMore = model.Offset + model.Take < changesCount
            };
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