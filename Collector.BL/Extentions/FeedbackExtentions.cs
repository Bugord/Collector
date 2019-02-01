using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Collector.BL.Models.Feedback;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class FeedbackExtentions
    {
        public static Feedback ToFeedback(this FeedbackAddDTO model, User creator)
        {
            return new Feedback
            {
                Creator = creator,
                Subject = model.Subject,
                Description = model.Description,
                IsClosed = false,
                CreatedBy = creator.Id,
                Messages = new List<FeedbackMessage>()
            };
        }

        public static FeedbackReturnDTO ToFeedbackReturnDTO(this Feedback feedback)
        {
            return new FeedbackReturnDTO
            {
                Id = feedback.Id,
                Description = feedback.Description,
                Subject = feedback.Subject,
                isClosed = feedback.IsClosed,
                Created = feedback.Created,
                Closed = feedback.Closed,
                ClosedBy = feedback.ClosedBy?.UserToUserReturnDTO(),
                Creator = feedback.Creator?.UserToUserReturnDTO(),
                MessagesCount = feedback.Messages.Count
            };
        }
    }
}
