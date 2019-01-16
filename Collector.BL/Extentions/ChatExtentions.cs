using System;
using System.Collections.Generic;
using System.Text;
using Collector.BL.Models.Chat;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class ChatExtentions
    {
        public static ChatReturnDTO ToChatReturnDTO(this ChatMessage chatMessage, long ownerId)
        {
            return new ChatReturnDTO
            {
                Username = chatMessage.Author.Username,
                Text = chatMessage.Text,
                SentTo = chatMessage.SentTo?.Username,
                IsOwner = chatMessage.Author.Id == ownerId,
                IsPrivate = chatMessage.SentTo != null,
                Created = chatMessage.Created
            };
        }
    }
}
