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
        Task <FeedbackMessageReturnDTO>AddFeedbackMessageAsync(FeedbackMessageAddDTO model);
        Task<FeedbackReturnDTO> GetFeedbackAsync(long id);
        Task<IList<FeedbackMessageReturnDTO>> GetFeedbackMessagesAsync(long id);
        Task<IList<FeedbackReturnDTO>> GetFeedbacksAsync(int offset, int count);
        Task<FeedbackReturnDTO> CloseFeedbackAsync(long id);
    }
}