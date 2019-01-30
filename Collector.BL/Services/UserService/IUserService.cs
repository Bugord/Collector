using System.Threading.Tasks;
using Collector.BL.Models.Authorization;

namespace Collector.BL.Services.UserService
{
    public interface IUserService
    {
        Task<UserReturnDTO> GetUserInfoAsync(long? id);
        Task ResetPasswordAsync(string email);
        Task ResetPasswordTokenAsync(ResetPasswordDTO model);
        Task ChangePasswordAsync(ChangePasswordDTO model);
        Task<UserReturnDTO> ChangeProfileAsync(ChangeProfileDTO model);
        Task ConfirmEmailAsync(string token);
    }
}