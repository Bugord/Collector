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
        Task<FeedbackReturnDTO> GetFeedback(long id);
        Task<IList<FeedbackMessageReturnDTO>> GetFeedbackMessages(long id);
        Task<IList<FeedbackReturnDTO>> GetFeedbacks(int offset, int count);
        Task<FeedbackReturnDTO> CloseFeedback(long id);
    }
}