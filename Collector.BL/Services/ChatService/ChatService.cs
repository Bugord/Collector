using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Extentions;
using Collector.BL.Models.Chat;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Collector.BL.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<ChatMessage> _chatMessagesRepository;
        private readonly IRepository<User> _usersRepository;

        public ChatService(IHttpContextAccessor httpContextAccessor, IRepository<ChatMessage> chatMessagesRepository,
            IRepository<User> usersRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _chatMessagesRepository = chatMessagesRepository;
            _usersRepository = usersRepository;
        }

        public async Task<IList<ChatReturnDTO>> GetAllChatMessages(ChatGetMessagesDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            if (!string.IsNullOrWhiteSpace(model.ChatWithUsername))
            {
                var chatWithUser =
                    await _usersRepository.GetFirstAsync(user => user.Username == model.ChatWithUsername);
                if (chatWithUser == null)
                {
                    throw new ArgumentException("User with such username does not exists");
                }


                var chatMessages = await _chatMessagesRepository.GetAllAsync(message =>
                        (message.Author.Id == ownerId && message.SentTo.Id == chatWithUser.Id)
                        || (message.Author.Id == chatWithUser.Id && message.SentTo.Id == ownerId),
                    message => message.Author, message => message.SentTo);

                return await chatMessages.Select(message => message.ToChatReturnDTO(ownerId)).ToListAsync();
            }

            return await (await _chatMessagesRepository.GetAllAsync(message => message.SentTo == null, message => message.Author))
                .Select(message => message.ToChatReturnDTO(ownerId)).ToListAsync();
        }
    }
}