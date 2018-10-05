using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Security.Claims;
using System.Threading.Tasks;
using Collector.BL.Exceptions;
using Collector.BL.Extentions;
using Collector.BL.Models.FriendList;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Http;

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
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var friendToAdd = new Friend()
            {
                OwnersName = model.Name,
                OwnerId = ownerId,
                CreatedBy = ownerId,
            };

            return (await _friendRepository.InsertAsync(friendToAdd)).FriendToFriendReturnDTO();
        }

        public async Task UpdateFriendAsync(FriendUpdateDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var oldFriend = await _friendRepository.GetByIdAsync(model.Id);
            if (oldFriend == null)
                throw new SqlNullValueException("Friend does not exist");

            if (oldFriend.OwnerId == ownerId)
                oldFriend.OwnersName = model.Name;
            else if (oldFriend.UserId == ownerId)
                oldFriend.UsersName = model.Name;
            else throw new NoPermissionException("You are not member of this friend connection");

            oldFriend.ModifiedBy = ownerId;
            await _friendRepository.UpdateAsync(oldFriend);
        }

        public async Task RemoveFriendAsync(long friendId)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var oldFriends = await _friendRepository.GetByIdAsync(friendId);
            if (oldFriends == null)
                throw new SqlNullValueException("Friend does not exist");

            var isOwnerFriend = oldFriends.OwnerId == ownerId;

            if (oldFriends.IsSynchronized && oldFriends.UserId != null)
            {
                var debts = await _debtRepository.GetAllAsync(debt => debt.FriendId == oldFriends.Id);

                foreach (var debtToChange in debts)
                {
                    if (debtToChange.OwnerId == ownerId)
                        debtToChange.IsOwnerDebter = !debtToChange.IsOwnerDebter;
                    
                    debtToChange.OwnerId = isOwnerFriend ? oldFriends.UserId.Value : oldFriends.OwnerId;

                    debtToChange.ModifiedBy = ownerId;
                }

                await _debtRepository.SaveChangesAsync();
            }

            if (isOwnerFriend)
            {
                if (oldFriends.IsSynchronized)
                {
                    oldFriends.Desync();
                    oldFriends.ModifiedBy = ownerId;
                    await _friendRepository.UpdateAsync(oldFriends);
                }
                else
                    await _friendRepository.RemoveByIdAsync(friendId);

                var invite = await _inviteRepository.GetFirstAsync(invite1 => invite1.FriendId == friendId);

                if (invite != null)
                    await _inviteRepository.RemoveAsync(invite);
            }

            var isUserFriend = oldFriends.UserId == ownerId;

            if (isUserFriend)
            {
                oldFriends.ClearUser();
                oldFriends.ModifiedBy = ownerId;
                await _friendRepository.UpdateAsync(oldFriends);
            }

            if (!isOwnerFriend && !isUserFriend)
                throw new NoPermissionException("User is not owner of this friend");
        }

        public async Task InviteFriendAsync(FriendInviteDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var isThisUser =
                await _userRepository.ExistsAsync(user =>
                    user.Id == ownerId && (user.Email == model.FriendEmail || user.Username == model.FriendEmail));
            if (isThisUser)
                throw new Exception("You can't invite yourself");

            var isOwnerFriend = await _friendRepository.ExistsAsync(friend =>
                friend.Id == model.FriendId && friend.OwnerId == ownerId);
            if (!isOwnerFriend)
                throw new NoPermissionException("User is not owner of this friend");

            var isAlreadySentFriend =
                await _inviteRepository.ExistsAsync(invite =>
                    invite.FriendId == model.FriendId && invite.OwnerId == ownerId);
            if (isAlreadySentFriend)
                throw new AlreadyExistsException("Invite of this friend already sent");

            var friendToInvite = await _userRepository.GetFirstAsync(user =>
                user.Email == model.FriendEmail || user.Username == model.FriendEmail);
            if (friendToInvite == null)
                throw new SqlNullValueException("User with this email does not exist");

            var isAlreadySentUser = await _inviteRepository.ExistsAsync(invite =>
                invite.UserId == friendToInvite.Id && invite.OwnerId == ownerId);
            if (isAlreadySentUser)
                throw new AlreadyExistsException("Invite to this user already sent");

            var isAlreadyFriends = await _friendRepository.ExistsAsync(friend =>
                friend.OwnerId == ownerId && friend.UserId == friendToInvite.Id
                || friend.OwnerId == friendToInvite.Id && friend.UserId == ownerId);
            if (isAlreadyFriends)
                throw new AlreadyExistsException("You already friends");


            var newInvite = model.FriendInviteDTOToInvite(friendToInvite, ownerId);

            var inviteRet = await _inviteRepository.InsertAsync(newInvite);

            var oldFriend = await _friendRepository.GetByIdAsync(model.FriendId);
            oldFriend.InviteId = inviteRet.Id;
            oldFriend.ModifiedBy = ownerId;
            await _friendRepository.UpdateAsync(oldFriend);
        }

        public async Task<IList<InviteReturnDTO>> GetAllInvitesAsync()
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var inviteList = await _inviteRepository.GetAllAsync(invite => invite.UserId == ownerId);
            var inviteReturnList = new List<InviteReturnDTO>();
            foreach (var invite in inviteList)
            {
                var ownerUser = await _userRepository.GetByIdAsync(invite.OwnerId);
                inviteReturnList.Add(invite.InviteToInviteReturnDTO(ownerUser));
            }

            return inviteReturnList;
        }

        public async Task<IList<FriendReturnDTO>> GetAllFriendsAsync()
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var friendList =
                await _friendRepository.GetAllAsync(friend => friend.OwnerId == ownerId || friend.UserId == ownerId);

            var friendReturnList = new List<FriendReturnDTO>();
            foreach (var friend in friendList)
            {
                if (friend.IsSynchronized && friend.UserId != null)
                {
                    var syncUser = friend.OwnerId == ownerId
                        ? await _userRepository.GetByIdAsync(friend.UserId.Value)
                        : await _userRepository.GetByIdAsync(friend.OwnerId);
                    if (syncUser == null)
                        continue;
                    friendReturnList.Add(friend.FriendToFriendReturnDTO(friend.OwnerId == ownerId, syncUser));
                }
                else
                    friendReturnList.Add(friend.FriendToFriendReturnDTO());
            }

            return friendReturnList;
        }

        public async Task ApproveInviteAsync(FriendAcceptDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

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
                        friend.OwnerId == oldInvite.OwnerId && friend.InviteId == model.InviteId);
                if(oldFriend == null)
                    throw new SqlNullValueException("Friend not founded");

                if (model.FriendId != null)
                {
                    var friendToSync = await _friendRepository.GetFirstAsync(friend =>
                        friend.OwnerId == ownerId && friend.Id == model.FriendId);

                    if (friendToSync == null)
                        throw new NoPermissionException("User is not owner of this friend");

                    var debtsToUpdate = await _debtRepository.GetAllAsync(debt => debt.FriendId == friendToSync.Id);

                    foreach (var debt in debtsToUpdate)
                    {
                        debt.FriendId = oldFriend.Id;
                        debt.ModifiedBy = ownerId;
                        await _debtRepository.UpdateAsync(debt);
                    }

                    oldFriend.UsersName = friendToSync.OwnersName;

                    await _friendRepository.RemoveAsync(friendToSync);
                }
                else
                    oldFriend.UsersName = model.UsersName;


                oldFriend.IsSynchronized = true;
                oldFriend.UserId = ownerId;
                oldFriend.ModifiedBy = ownerId;
                await _friendRepository.UpdateAsync(oldFriend);
            }

            await _inviteRepository.RemoveByIdAsync(model.InviteId);
        }
    }
}