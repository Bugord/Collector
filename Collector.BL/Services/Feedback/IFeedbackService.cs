using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Models.Feedback;

namespace Collector.BL.Services.Feedback
{
    public interface IFeedbackService
    {
        Task<FeedbackReturnDTO> AddFeedbackAsync(FeedbackAddDTO model);
        Task AddFeedbackMessageAsync(FeedbackMessageAddDTO model);
        Task<FeedbackReturnDTO> GetFeedback(long id);
        Task CloseFeedback(long id);
    }
}
