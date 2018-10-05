using Collector.BL.Models.FriendList;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class FriendExtentions
    {
        public static FriendReturnDTO FriendToFriendReturnDTO(this Friend friend, bool isOwner = true, User user = null)
        {
            var tempFriend = new FriendReturnDTO
            {
                Id = friend.Id,
                IsSynchronized = friend.IsSynchronized,
                Name = friend.OwnersName
            };

            if (user == null) return tempFriend;

            tempFriend.Email = user.Email;
            tempFriend.FirstName = user.FirstName;
            tempFriend.LastName = user.LastName;
            tempFriend.Name = isOwner ? friend.OwnersName : friend.UsersName;
            tempFriend.Username = user.Username;

            return tempFriend;
        }

        public static Friend Desync(this Friend friend)
        {
            if (friend.UserId == null || !friend.IsSynchronized) return friend;

            friend.OwnerId = friend.UserId.Value;
            friend.OwnersName = friend.UsersName;
            friend.IsSynchronized = false;
            friend.UsersName = null;
            friend.UserId = null;

            return friend;
        }

        public static Friend ClearUser(this Friend friend)
        {
            friend.IsSynchronized = false;
            friend.UserId = null;
            friend.InviteId = null;
            friend.UsersName = null;

            return friend;
        }
    }
}