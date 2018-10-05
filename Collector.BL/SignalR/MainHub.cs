using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Collector.BL.SignalR
{
    public class MainHub : Hub
    {
        public async Task UpdateInvites(string username)
        {
            await Clients.Others.SendAsync("UpdateInvites", username);
        }
        public async Task UpdateDebts(string username)
        {
            await Clients.Others.SendAsync("UpdateDebts", username);
        }
    }
}
