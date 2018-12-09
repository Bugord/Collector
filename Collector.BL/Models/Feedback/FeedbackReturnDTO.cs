using System;
using System.Collections.Generic;
using Collector.BL.Models.Authorization;
using Collector.DAO.Entities;

namespace Collector.BL.Models.Feedback
{
    public class FeedbackReturnDTO
    {
        public long Id { get; set; }
        public UserReturnDTO Creator { get; set; }
        public DateTime Created { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public bool isClosed { get; set; }
        public UserReturnDTO ClosedBy { get; set; }
        public DateTime? Closed { get; set; }
        public int MessagesCount { get; set; }
    }
}
