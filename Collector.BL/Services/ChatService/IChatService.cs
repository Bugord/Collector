using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Models.Chat;

namespace Collector.BL.Services.ChatService
{
    public interface IChatService
    {
        Task<IList<ChatReturnDTO>> GetAllChatMessages(ChatGetMessagesDTO model);
    } 
}
