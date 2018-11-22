using Collector.BL.Models.FriendList;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class FriendExtentions
    {
        public static FriendReturnDTO FriendToFriendReturnDTO(this Friend friend)
        {
            return new FriendReturnDTO
            {
                Id = friend.Id,
                Name = friend.Name,
                IsSynchronized = friend.IsSynchronized,
                FriendUser = friend.FriendUser?.UserToUserReturnDTO()
            };
        }

        public static Friend ClearUser(this Friend friend)
        {
            friend.IsSynchronized = false;
            friend.FriendUser = null;
            friend.InviteId = null;

            return friend;
        }
    }
}