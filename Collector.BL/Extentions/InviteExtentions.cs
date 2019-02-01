using Collector.BL.Models.FriendList;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class InviteExtentions
    {
        public static InviteReturnDTO InviteToInviteReturnDTO(this Invite invite)
        {
            return new InviteReturnDTO
            {
                Id = invite.Id,
                Email = invite.OwnerUser.Email,
                Username = invite.OwnerUser.Username,
                LastName = invite.OwnerUser.LastName,
                FirstName = invite.OwnerUser.FirstName,
            };
        }

        public static Invite FriendInviteDTOToInvite(this FriendInviteDTO model, User friendToInvite, long ownerId, User ownerUser)
        {
            return new Invite
            {
                Approved = false,
                FriendId = model.FriendId,
                CreatedBy = ownerId,
                UserId = friendToInvite.Id,
                OwnerId = ownerId,
                OwnerUser = ownerUser
            };
        }
    }
}
