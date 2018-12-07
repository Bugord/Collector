using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Extentions;
using Collector.BL.Models.Feedback;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Collector.BL.Services.Feedback
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<DAO.Entities.Feedback> _feedbackRepository;
        private readonly IRepository<FeedbackMessage> _feedbackMessagesRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FeedbackService(IRepository<User> userRepository,
            IHttpContextAccessor httpContextAccessor, IRepository<DAO.Entities.Feedback> feedbackRepository,
            IRepository<FeedbackMessage> feedbackMessagesRepository)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _feedbackRepository = feedbackRepository;
            _feedbackMessagesRepository = feedbackMessagesRepository;
        }

        public async Task<IList<FeedbackMessageReturnDTO>> GetFeedbackMessages(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var roleClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
            if (roleClaim == null)
                throw new UnauthorizedAccessException();
            if (!Enum.TryParse(roleClaim.Value, out Role ownerRole))
                throw new UnauthorizedAccessException();

            var isAdminOrModerator = ownerRole == Role.Admin || ownerRole == Role.Moderator;

            var feedbackMessages = await _feedbackMessagesRepository.GetAllAsync(message =>
                    message.Feedback.Id == id && (isAdminOrModerator || message.Feedback.Creator.Id == ownerId),
                message => message.Author);

            if (feedbackMessages == null)
                throw new UnauthorizedAccessException();

            return feedbackMessages.Select(message => message.ToFeedbackMessageReturnDTO()).ToList();
        }

        public async Task<IList<FeedbackReturnDTO>> GetFeedbacks(int offset, int count)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var roleClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
            if (roleClaim == null)
                throw new UnauthorizedAccessException();
            if (!Enum.TryParse(roleClaim.Value, out Role ownerRole))
                throw new UnauthorizedAccessException();

            var isAdminOrModerator = ownerRole == Role.Admin || ownerRole == Role.Moderator;

            var feedbacks = await (await _feedbackRepository.GetAllAsync(
                    feedback => isAdminOrModerator || feedback.Creator.Id == ownerId, feedback => feedback.Creator,
                    feedback => feedback.ClosedBy))
                .Select(feedback => feedback.ToFeedbackReturnDTO(feedback.Messages.Count)).ToListAsync();

            return feedbacks;
        }

        public async Task<FeedbackReturnDTO> AddFeedbackAsync(FeedbackAddDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var creator = await _userRepository.GetByIdAsync(ownerId);
            var newFeedback = model.ToFeedback(creator);
            var addedFeedback = await _feedbackRepository.InsertAsync(newFeedback);
            return addedFeedback.ToFeedbackReturnDTO();
        }

        public async Task<FeedbackMessageReturnDTO> AddFeedbackMessageAsync(FeedbackMessageAddDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var creator = await _userRepository.GetByIdAsync(ownerId);
            var feedback = await _feedbackRepository.GetByIdAsync(model.FeedbackId, feedback1 => feedback1.Messages);
            var newFeedbackMessage = model.ToFeedbackMessage(feedback, creator);
            feedback.Messages.Add(newFeedbackMessage);
            await _feedbackRepository.UpdateAsync(feedback);
            return newFeedbackMessage.ToFeedbackMessageReturnDTO();
        }

        public async Task<FeedbackReturnDTO> GetFeedback(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var roleClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
            if (roleClaim == null)
                throw new UnauthorizedAccessException();
            Enum.TryParse(roleClaim.Value, out Role ownerRole);

            var isAdminOrModerator = ownerRole == Role.Admin || ownerRole == Role.Moderator;

            var feedbackToReturn = await _feedbackRepository.GetFirstAsync(
                feedback => (feedback.Creator.Id == ownerId || isAdminOrModerator) && feedback.Id == id,
                feedback => feedback.Creator,
                feedback => feedback.Messages, feedback => feedback.ClosedBy);

            if (feedbackToReturn == null)
                throw new UnauthorizedAccessException();


            return feedbackToReturn.ToFeedbackReturnDTO();
        }

        public async Task<FeedbackReturnDTO> CloseFeedback(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!long.TryParse(idClaim, out var ownerId))
                throw new UnauthorizedAccessException();

            var roleClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
            if (roleClaim == null)
                throw new UnauthorizedAccessException();
            Enum.TryParse(roleClaim.Value, out Role ownerRole);


            var isAdminOrModerator = ownerRole == Role.Admin || ownerRole == Role.Moderator;


            var feedbackToClose =
                await _feedbackRepository.GetFirstAsync(feedback =>
                        (isAdminOrModerator || feedback.Creator.Id == ownerId) && feedback.Id == id,
                    feedback => feedback.Creator, feedback => feedback.Messages);

            var closedBy = await _userRepository.GetByIdAsync(ownerId);
            if (closedBy == null)
                throw new UnauthorizedAccessException();

            feedbackToClose.IsClosed = true;
            feedbackToClose.ClosedBy = closedBy;
            feedbackToClose.Closed = DateTime.UtcNow;

            await _feedbackRepository.UpdateAsync(feedbackToClose);
            return feedbackToClose.ToFeedbackReturnDTO();
        }
    }
}