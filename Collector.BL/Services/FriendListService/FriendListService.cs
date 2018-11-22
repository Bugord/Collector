using System;
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
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

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
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var friend = await _friendRepository.GetByIdAsync(friendId, friend1 => friend1.FriendUser);
            if (friend.IsSynchronized)
            {
               

                var otherFriend = await _friendRepository.GetFirstAsync(friend1 =>
                    friend1.FriendUser.Id == ownerId && friend1.Owner.Id == friend.FriendUser.Id, friend1 => friend1.FriendUser, friend1 => friend1.Owner);

                var debts = await _debtRepository.GetAllAsync(debt => debt.Friend == friend);
                foreach (var debt in debts)
                {
                    debt.Friend = otherFriend;
                    debt.Owner = otherFriend.Owner;
                    debt.IsOwnerDebter = !debt.IsOwnerDebter;
                }

                otherFriend.IsSynchronized = false;
                otherFriend.FriendUser = null;

                await _friendRepository.UpdateAsync(otherFriend);
            }
            await _friendRepository.RemoveAsync(friend);

            //    var oldFriends = await _friendRepository.GetByIdAsync(friendId);
            //    if (oldFriends == null)
            //        throw new SqlNullValueException("Friend does not exist");

            //    var isOwnerFriend = oldFriends.Owner.Id == ownerId;

            //    if (oldFriends.IsSynchronized && oldFriends.UserId != null)
            //    {
            //        var debts = await _debtRepository.GetAllAsync(debt => debt.FriendId == oldFriends.Id);

            //        foreach (var debtToChange in debts)
            //        {
            //            if (debtToChange.OwnerId == ownerId)
            //                debtToChange.IsOwnerDebter = !debtToChange.IsOwnerDebter;

            //            debtToChange.OwnerId = isOwnerFriend ? oldFriends.UserId.Value : oldFriends.OwnerId;

            //            debtToChange.ModifiedBy = ownerId;
            //        }

            //        await _debtRepository.SaveChangesAsync();
            //    }

            //    if (isOwnerFriend)
            //    {
            //        if (oldFriends.IsSynchronized)
            //        {
            //            oldFriends.Desync();
            //            oldFriends.ModifiedBy = ownerId;
            //            await _friendRepository.UpdateAsync(oldFriends);
            //        }
            //        else
            //            await _friendRepository.RemoveByIdAsync(friendId);

            //        var invite = await _inviteRepository.GetFirstAsync(invite1 => invite1.FriendId == friendId);

            //        if (invite != null)
            //            await _inviteRepository.RemoveAsync(invite);
            //    }

            //    var isUserFriend = oldFriends.UserId == ownerId;

            //    if (isUserFriend)
            //    {
            //        oldFriends.ClearUser();
            //        oldFriends.ModifiedBy = ownerId;
            //        await _friendRepository.UpdateAsync(oldFriends);
            //    }

            //    if (!isOwnerFriend && !isUserFriend)
            //        throw new NoPermissionException("User is not owner of this friend");
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
                user.Email == model.FriendEmail || user.Username == model.FriendEmail);
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
                (await _friendRepository.GetAllAsync(friend =>
                    friend.Owner.Id == ownerId, friend => friend.FriendUser))
                .Select(friend => friend.FriendToFriendReturnDTO()).ToList();

            //var friendReturnList = new List<FriendReturnDTO>();
            //foreach (var friend in friendList)
            //{
            //    if (friend.IsSynchronized && friend.FriendUser != null)
            //    {
            //        var syncUser = friend.Owner.Id == ownerId
            //            ? await _userRepository.GetByIdAsync(friend.FriendUser.Id)
            //            : await _userRepository.GetByIdAsync(friend.Owner.Id);
            //        if (syncUser == null)
            //            continue;
            //        friendReturnList.Add(friend.FriendToFriendReturnDTO());
            //    }
            //    else
            //        friendReturnList.Add(friend.FriendToFriendReturnDTO());
            //}

            return friendList;
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
                        friend.Owner.Id == oldInvite.OwnerId && friend.InviteId == model.InviteId);
                if (oldFriend == null)
                    throw new SqlNullValueException("Friend not founded");

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
                    //            var debtsToUpdate = await _debtRepository.GetAllAsync(debt => debt.FriendId == friendToSync.Id);

                    //            foreach (var debt in debtsToUpdate)
                    //            {
                    //                debt.FriendId = oldFriend.Id;
                    //                debt.ModifiedBy = ownerId;
                    //                //await _debtRepository.UpdateAsync(debt);
                    //            }

                    //            await _debtRepository.SaveChangesAsync();
                    //            oldFriend.UsersName = friendToSync.OwnersName;

                    //            await _friendRepository.RemoveAsync(friendToSync);
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
                //        else
                //            oldFriend.UsersName = model.UsersName;


                //        oldFriend.IsSynchronized = true;
                //        oldFriend.UserId = ownerId;
                //        oldFriend.ModifiedBy = ownerId;
                //        await _friendRepository.UpdateAsync(oldFriend);
            }

                await _inviteRepository.RemoveByIdAsync(model.InviteId);
        }
    }
}