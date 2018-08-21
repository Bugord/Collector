using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Entity;

namespace Collector.BL.Authorization
{
    public interface ITokenService
    {
        Task<object> RegisterAsync(RegisterDto entity);
        Task<object> LoginAsync(LoginDto entity);

    }
}
