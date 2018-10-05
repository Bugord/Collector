using Collector.BL.Models.FriendList;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class InviteExtentions
    {
        public static InviteReturnDTO InviteToInviteReturnDTO(this Invite invite, User owner)
        {
            return new InviteReturnDTO
            {
                Id = invite.Id,
                Email = owner.Email,
                Username = owner.Username,
                LastName = owner.LastName,
                FirstName = owner.FirstName,
            };
        }

        public static Invite FriendInviteDTOToInvite(this FriendInviteDTO model, User friendToInvite, long ownerId)
        {
            return new Invite
            {
                Approved = false,
                FriendId = model.FriendId,
                CreatedBy = ownerId,
                UserId = friendToInvite.Id,
                OwnerId = ownerId
            };
        }
    }
}
