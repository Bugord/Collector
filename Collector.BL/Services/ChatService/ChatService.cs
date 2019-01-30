using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Collector.BL.Extentions;
using Collector.BL.Models.Chat;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Collector.BL.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<ChatMessage> _chatMessageRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _configuration;

        public ChatService(IHttpContextAccessor httpContextAccessor, IRepository<ChatMessage> chatMessageRepository,
            IRepository<User> userRepository, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _chatMessageRepository = chatMessageRepository;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<IList<ChatMessageReturnDTO>> GetAllChatMessagesAsync(ChatMessagesGetDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            if (!string.IsNullOrWhiteSpace(model.ChatWithUsername))
            {
                var chatWithUser =
                    await _userRepository.GetFirstAsync(user => user.Username == model.ChatWithUsername);
                if (chatWithUser == null)
                {
                    throw new ArgumentException("User with such username does not exists");
                }


                var chatMessages = (await _chatMessageRepository.GetAllAsync(message =>
                            (message.Author.Id == ownerId && message.SentTo.Id == chatWithUser.Id)
                            || (message.Author.Id == chatWithUser.Id && message.SentTo.Id == ownerId),
                        message => message.Author, message => message.SentTo))
                    .OrderByDescending(message => message.Created)
                    .Skip(model.SkipMessages)
                    .Take(model.TakeMessages)
                    .OrderBy(message => message.Created);

                return await chatMessages.Select(message => message.ToChatReturnDTO(ownerId)).ToListAsync();
            }

            return await (await _chatMessageRepository.GetAllAsync(message => message.SentTo == null,
                    message => message.Author))
                .OrderByDescending(message => message.Created)
                .Skip(model.SkipMessages)
                .Take(model.TakeMessages)
                .OrderBy(message => message.Created)
                .Select(message => message.ToChatReturnDTO(ownerId))
                .ToListAsync();
        }

        public async Task<ChatMessageReturnDTO> SendChatMessageAsync(ChatMessageAddDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var ownerUser = await _userRepository.GetByIdAsync(ownerId);
            if (ownerUser == null)
                throw new UnauthorizedAccessException();

            User sentToUser = null;
            if (!string.IsNullOrWhiteSpace(model.SendToUsername))
            {
                sentToUser = await _userRepository.GetFirstAsync(user =>
                    user.Username.Equals(model.SendToUsername, StringComparison.InvariantCultureIgnoreCase));
                if (sentToUser == null)
                    throw new ArgumentException("User with this username does not exist");
            }

            var newChatMessage = new ChatMessage
            {
                CreatedBy = ownerUser.Id,
                Author = ownerUser,
                Text = model.MessageText,
                SentTo = sentToUser,
                Type = await DefineChatMessageTypeAsync(model.MessageText)
            };
            var chatMessage = await _chatMessageRepository.InsertAsync(newChatMessage);

            return chatMessage.ToChatReturnDTO(ownerId);

            //if (string.IsNullOrWhiteSpace(model.SendToUsername))
            //    await Clients.Others.SendAsync("MessageReceived",
            //        new { user.Username, text, isPrivate = false, created = newChatMessage.Created, AvatarUrl = user.AratarUrl });
            //else
            //    await Clients.User(sentToUser?.Id.ToString()).SendAsync("MessageReceived",
            //        new { user.Username, text, isPrivate = true, sentTo, created = newChatMessage.Created, AvatarUrl = user.AratarUrl });
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var ownerUser = await _userRepository.GetByIdAsync(ownerId);

            if (file != null && file.Length != 0)
            {
                var fileName = (ownerUser.Username + "_" + DateTime.UtcNow.ToFileTimeUtc()).CreateMd5() +
                               Path.GetExtension(file.FileName);
                var folderPath = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot", _configuration["ChatUploadPath"], ownerUser.Username
                );

                var filePath = Path.Combine(
                    folderPath, fileName
                );
                Directory.CreateDirectory(folderPath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Path.Combine("/uploads", ownerUser.Username, fileName);
            }

            throw new FileLoadException("File is empty");
        }

        public async Task<ChatMessageType> DefineChatMessageTypeAsync(string text)
        {
            var isUrlLink = Uri.TryCreate(text, UriKind.Absolute, out var uriResult)
                            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (isUrlLink)
            {
                var youtubeRegex =
                    new Regex(
                        @"^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$");
                if (youtubeRegex.IsMatch(text))
                    return ChatMessageType.YoutubeVideo;

                var coubRegex =
                    new Regex(
                        @"^((?:https?:)?\/\/)?((?:www|m)\.)?((coub\.com))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$");
                if (coubRegex.IsMatch(text))
                    return ChatMessageType.Coub;

                var req = (HttpWebRequest) WebRequest.Create(text);
                req.Method = "HEAD";
                try
                {
                    using (var resp = await req.GetResponseAsync())
                    {
                        var contentType = resp.ContentType.ToLower(CultureInfo.InvariantCulture);
                        if (contentType.StartsWith("image/"))
                            return ChatMessageType.Image;
                        if (contentType.StartsWith("audio/"))
                            return ChatMessageType.Audio;
                    }
                }
                catch (Exception e)
                {
                    return ChatMessageType.Url;
                }


                return ChatMessageType.Url;
            }

            return ChatMessageType.Text;
        }
    }
}