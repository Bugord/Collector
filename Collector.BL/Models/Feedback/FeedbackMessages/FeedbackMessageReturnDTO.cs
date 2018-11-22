using System;
using System.Collections.Generic;
using System.Text;
using Collector.BL.Models.Authorization;

namespace Collector.BL.Models.Feedback
{
    public class FeedbackMessageReturnDTO
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public UserReturnDTO User { get; set; }
        public DateTime Created { get; set; }
    }
}
