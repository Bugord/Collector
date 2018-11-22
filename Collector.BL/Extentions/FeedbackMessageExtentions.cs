using System;
using System.Collections.Generic;
using System.Text;
using Collector.BL.Models.Feedback;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class FeedbackMessageExtentions
    {
        public static FeedbackMessage ToFeedbackMessage(this FeedbackMessageAddDTO model, Feedback feedback, User author)
        {
            return new FeedbackMessage
            {
                Author = author,
                Text = model.Text,
                Feedback = feedback,
                CreatedBy = author.Id
            };
        }

        public static FeedbackMessageReturnDTO ToFeedbackMessageReturnDTO(this FeedbackMessage message)
        {
            return new FeedbackMessageReturnDTO
            {
                User = message.Author.UserToUserReturnDTO(),
                Id = message.Id,
                Text = message.Text,
                Created = message.Created
            };
        }
    }
}
