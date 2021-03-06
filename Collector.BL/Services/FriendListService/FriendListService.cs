﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Collector.BL.Exceptions;
using Collector.BL.Extentions;
using Collector.BL.Models.FriendList;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Collector.BL.Services.FriendListService
{
    public class FriendListService : IFriendListService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Friend> _friendRepository;
        private readonly IRepository<Invite> _inviteRepository;
        private readonly IRepository<Debt> _debtRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FriendListService(IRepository<User> userRepository, IRepository<Friend> friendRepository,
            IRepository<Invite> inviteRepository, IRepository<Debt> debtRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _friendRepository = friendRepository;
            _inviteRepository = inviteRepository;
            _debtRepository = debtRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<FriendReturnDTO> AddFriendAsync(FriendAddDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var owner = await _userRepository.GetByIdAsync(ownerId);

            var friendToAdd = new Friend()
            {
                Name = model.Name,
                Owner = owner,
                CreatedBy = owner.Id,
            };

            return (await _friendRepository.InsertAsync(friendToAdd)).FriendToFriendReturnDTO();
        }

        public async Task UpdateFriendAsync(FriendUpdateDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var oldFriend = await _friendRepository.GetByIdAsync(model.Id, friend => friend.Owner);
            if (oldFriend == null)
                throw new SqlNullValueException("Friend does not exist");

            if (oldFriend.Owner.Id == ownerId)
                oldFriend.Name = model.Name;
            else throw new NoPermissionException("You are not member of this friend connection");

            oldFriend.ModifiedBy = ownerId;
            await _friendRepository.UpdateAsync(oldFriend);
        }

        public async Task RemoveFriendAsync(long friendId)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var friend = await _friendRepository.GetByIdAsync(friendId, friend1 => friend1.FriendUser);
            if (friend.IsSynchronized)
            {
                var otherFriend = await _friendRepository.GetFirstAsync(friend1 =>
                        friend1.FriendUser.Id == ownerId && friend1.Owner.Id == friend.FriendUser.Id,
                    friend1 => friend1.FriendUser);

                otherFriend.IsSynchronized = false;
                otherFriend.FriendUser = null;

                await _friendRepository.UpdateAsync(otherFriend);
            }

            if (friend.InviteId.HasValue)
            {
                var oldInvite = await _inviteRepository.GetByIdAsync(friend.InviteId.Value);
                if (oldInvite != null)
                    await _inviteRepository.RemoveAsync(oldInvite);
            }

            await _friendRepository.RemoveAsync(friend);
        }

        public async Task InviteFriendAsync(FriendInviteDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var ownerUser = await _userRepository.GetByIdAsync(ownerId);

            var isThisUser =
                await _userRepository.ExistsAsync(user =>
                    user.Id == ownerId && (user.Email == model.FriendEmail || user.Username == model.FriendEmail));
            if (isThisUser)
                throw new ArgumentException("You can't invite yourself");

            var isOwnerFriend = await _friendRepository.ExistsAsync(friend =>
                friend.Id == model.FriendId && friend.Owner.Id == ownerId);
            if (!isOwnerFriend)
                throw new NoPermissionException("User is not owner of this friend");

            var isAlreadySentFriend =
                await _inviteRepository.ExistsAsync(invite =>
                    invite.FriendId == model.FriendId && invite.OwnerId == ownerId);
            if (isAlreadySentFriend)
                throw new AlreadyExistsException("Invite of this friend already sent");

            var friendToInvite = await _userRepository.GetFirstAsync(user =>
                user.Email.ToUpper().Equals(model.FriendEmail.ToUpper()));
            if (friendToInvite == null)
                throw new SqlNullValueException("User with this email does not exist");

            var isAlreadySentUser = await _inviteRepository.ExistsAsync(invite =>
                invite.UserId == friendToInvite.Id && invite.OwnerId == ownerId);
            if (isAlreadySentUser)
                throw new AlreadyExistsException("Invite to this user already sent");

            var isAlreadyFriends = await _friendRepository.ExistsAsync(friend =>
                friend.Owner.Id == ownerId && friend.FriendUser.Id == friendToInvite.Id
                || friend.Owner.Id == friendToInvite.Id && friend.FriendUser.Id == ownerId);
            if (isAlreadyFriends)
                throw new AlreadyExistsException("You already friends");


            var newInvite = model.FriendInviteDTOToInvite(friendToInvite, ownerId, ownerUser);

            var inviteRet = await _inviteRepository.InsertAsync(newInvite);

            var oldFriend = await _friendRepository.GetByIdAsync(model.FriendId);
            oldFriend.InviteId = inviteRet.Id;
            oldFriend.ModifiedBy = ownerId;
            await _friendRepository.UpdateAsync(oldFriend);
        }

        public async Task<IList<InviteReturnDTO>> GetAllInvitesAsync()
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var inviteList = await (await _inviteRepository.GetAllAsync(invite => invite.UserId == ownerId,
                    invite => invite.OwnerUser))
                .Select(invite => invite.InviteToInviteReturnDTO())
                .ToListAsync();

            return inviteList;
        }

        public async Task<IList<FriendReturnDTO>> GetAllFriendsAsync()
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var friendList =
                (await _friendRepository.GetAllAsync(friend =>
                    friend.Owner.Id == ownerId, friend => friend.FriendUser))
                //.Select(friend => friend.FriendToFriendReturnDTO())
                .ToList();

            var debts = await (await _debtRepository.GetAllAsync(debt =>
                    !debt.IsClosed && debt.IsMoney && ((debt.Owner.Id == ownerId) ||
                    debt.Friend.FriendUser.Id == ownerId && debt.Synchronize))).Include(debt => debt.Owner)
                .Include(debt => debt.Friend).ThenInclude(friend => friend.FriendUser).ToListAsync();

            var friendRet = friendList.Select(friend => friend.FriendToFriendReturnDTO()).ToList();
            
            foreach (var friend in friendList)
            {
                decimal toOnwer = 0;
                decimal fromOnwer = 0;

                if(friend.IsSynchronized)
                foreach (var debt in debts.Where(debt =>
                    debt.Owner.Id == friend.FriendUser.Id))
                {
                    if (debt.IsOwnerDebter)
                        toOnwer += debt.Value.Value;
                    else fromOnwer += debt.Value.Value;
                }

                foreach (var debt in debts.Where(debt =>
                    debt.Friend.Id == friend.Id))
                {
                    if (debt.IsOwnerDebter)
                        fromOnwer += debt.Value.Value;
                    else toOnwer += debt.Value.Value;
                }

                var test = friendRet.FirstOrDefault(dto => dto.Id == friend.Id);
                test.Owe1 = toOnwer;
                test.Owe2 = fromOnwer;
            }

            return friendRet;
        }

        public async Task ApproveInviteAsync(FriendAcceptDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var isOwnerFriend = await _inviteRepository.ExistsAsync(invite =>
                invite.Id == model.InviteId && invite.UserId == ownerId);

            if (!isOwnerFriend)
                throw new NoPermissionException("User is not owner of this invite");

            if (model.Accepted)
            {
                var oldInvite =
                    await _inviteRepository.GetByIdAsync(model.InviteId);


                var oldFriend =
                    await _friendRepository.GetFirstAsync(friend =>
                        friend.Owner.Id == oldInvite.OwnerId && friend.InviteId == model.InviteId);
                if (oldFriend == null)
                    throw new SqlNullValueException("Friend not found");

                if (model.FriendId != null)
                {
                    var friendToSync = await _friendRepository.GetFirstAsync(friend =>
                        friend.Owner.Id == ownerId && friend.Id == model.FriendId);

                    if (friendToSync == null)
                        throw new NoPermissionException("Friend not Founded");

                    friendToSync.IsSynchronized = true;
                    friendToSync.FriendUser = await _userRepository.GetByIdAsync(oldInvite.OwnerId);

                    oldFriend.IsSynchronized = true;
                    oldFriend.FriendUser = await _userRepository.GetByIdAsync(ownerId);

                    await _friendRepository.UpdateAsync(friendToSync);
                    await _friendRepository.UpdateAsync(oldFriend);
                }
                else
                {
                    var addFriendDTO = new FriendAddDTO
                    {
                        Name = model.UsersName
                    };

                    var addedFriend = await AddFriendAsync(addFriendDTO);

                    var newFriend = await _friendRepository.GetByIdAsync(addedFriend.Id);

                    newFriend.IsSynchronized = true;
                    newFriend.FriendUser = await _userRepository.GetByIdAsync(oldInvite.OwnerId);

                    oldFriend.IsSynchronized = true;
                    oldFriend.FriendUser = await _userRepository.GetByIdAsync(ownerId);

                    await _friendRepository.UpdateAsync(newFriend);
                    await _friendRepository.UpdateAsync(oldFriend);
                }
            }

            await _inviteRepository.RemoveByIdAsync(model.InviteId);
        }
    }
}