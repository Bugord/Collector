using System;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Collector.Attributes;
using Collector.BL.Services.EmailService;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Security.Certificates;

namespace Collector.BL.SignalR
{
    public class MainHub : Hub
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ChatMessage> _chatMessageRepository;

        public MainHub(IRepository<User> userRepository, IRepository<ChatMessage> chatMessageRepository)
        {
            _userRepository = userRepository;
            _chatMessageRepository = chatMessageRepository;
        }

        [Authorize]
        public async Task UpdateInvites(string username)
        {
            var userToInvite =
                await _userRepository.GetFirstAsync(user => user.Username == username || user.Email == username);
            if (userToInvite == null)
                throw new ArgumentException();

            var userId = userToInvite.Id;
            await Clients.User(userId.ToString()).SendAsync("UpdateInvites");
        }

        [Authorize]
        public async Task SendMessage(string message, string sentTo)
        {
            var idClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var user = await _userRepository.GetByIdAsync(ownerId);
            if (user == null)
                throw new ArgumentException();

            User sentToUser = null;
            if (!string.IsNullOrEmpty(sentTo))
            {
                sentToUser = await _userRepository.GetFirstAsync(user1 => user1.Username == sentTo);
                if (sentToUser == null)
                    throw new ArgumentException("User with this username does not exist");
            }

            var newChatMessage = new ChatMessage
            {
                CreatedBy = user.Id,
                Author = user,
                Text = message,
                SentTo = sentTo
            };
            await _chatMessageRepository.InsertAsync(newChatMessage);

            if (string.IsNullOrEmpty(sentTo))
                await Clients.Others.SendAsync("MessageReceived", user.Username, message, false);
            else await Clients.User(sentToUser?.Id.ToString()).SendAsync("MessageReceived", user.Username, message, true);
        }

        [Authorize]
        public async Task StartTyping()
        {
            var idClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var user = await _userRepository.GetByIdAsync(ownerId);
            if (user == null)
                throw new ArgumentException();
            
            await Clients.Others.SendAsync("StartTyping", user.Username);
        }

        [Authorize]
        public async Task StopTyping()
        {
            var idClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var user = await _userRepository.GetByIdAsync(ownerId);
            if (user == null)
                throw new ArgumentException();

            await Clients.Others.SendAsync("StopTyping", user.Username);
        }
    }
}