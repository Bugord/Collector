using System.Collections.Generic;
using System.Threading.Tasks;
using Collector.BL.Models.FriendList;

namespace Collector.BL.Services.FriendListService
{
    public interface IFriendListService
    {
        Task<FriendReturnDTO> AddFriendAsync(FriendAddDTO model);
        Task<IList<FriendReturnDTO>> GetAllFriendsAsync();
        Task RemoveFriendAsync(long id);
        Task InviteFriendAsync(FriendInviteDTO model);
        Task<IList<InviteReturnDTO>> GetAllInvitesAsync();
        Task ApproveInviteAsync(FriendAcceptDTO model);
        Task UpdateFriendAsync(FriendUpdateDTO model);
    }
}
