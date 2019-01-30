using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Models.Chat;
using Collector.DAO.Entities;
using Microsoft.AspNetCore.Http;

namespace Collector.BL.Services.ChatService
{
    public interface IChatService
    {
        Task<IList<ChatMessageReturnDTO>> GetAllChatMessagesAsync(ChatMessagesGetDTO model);
        Task<ChatMessageReturnDTO> SendChatMessageAsync(ChatMessageAddDTO model);
        Task<ChatMessageType> DefineChatMessageTypeAsync(string text);
        Task<string> UploadFileAsync(IFormFile file);
    } 
}
