using System.Threading.Tasks;
using Collector.BL.Models.Authorization;

namespace Collector.BL.Services.AuthorizationService
{
    public interface ITokenService
    {
        Task<AuthorizationReturnDTO> RegisterAsync(RegisterDTO model);
        
        Task<AuthorizationReturnDTO> LoginAsync(LoginDTO model);
    }
}
