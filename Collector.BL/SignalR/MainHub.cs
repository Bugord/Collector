using System;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Collector.Attributes;
using Collector.BL.Models.Chat;
using Collector.BL.Services.ChatService;
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
        private readonly IChatService _chatService;

        public MainHub(IRepository<User> userRepository, IRepository<ChatMessage> chatMessageRepository,
            IChatService chatService)
        {
            _userRepository = userRepository;
            _chatMessageRepository = chatMessageRepository;
            _chatService = chatService;
        }

        [Authorize]
        public async Task UpdateInvites(string username)
        {
            var userToInvite =
                await _userRepository.GetFirstAsync(user => user.Email == username);
            if (userToInvite == null)
                throw new ArgumentException();

            var userId = userToInvite.Id;
            await Clients.User(userId.ToString()).SendAsync("UpdateInvites");
        }

        [Authorize]
        public async Task SendMessage(string text, string sentTo, string tempId)
        {
            try
            {
                var chatMessage = await
                    _chatService.SendChatMessageAsync(new ChatMessageAddDTO
                    {
                        MessageText = text,
                        SendToUsername = sentTo
                    });

                if (string.IsNullOrEmpty(sentTo))
                    await Clients.Others.SendAsync("MessageReceived",
                        chatMessage);
                else
                {
                    var sentToUser = await _userRepository.GetFirstAsync(user =>
                        user.Username.Equals(sentTo, StringComparison.InvariantCultureIgnoreCase));

                    if (sentToUser == null)
                        throw new ArgumentException("User with such username does not exist");

                    await Clients.User(sentToUser.Id.ToString()).SendAsync("MessageReceived",
                        chatMessage);
                }

                await Clients.Caller.SendAsync("MessageApproved", true, tempId, chatMessage);
            }
            catch (Exception e)
            {
                await Clients.Caller.SendAsync("MessageApproved", false, tempId);
            }
        }
    }
}