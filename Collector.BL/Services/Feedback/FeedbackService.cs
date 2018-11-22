using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Collector.BL.Extentions;
using Collector.BL.Models.Feedback;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.AspNetCore.Http;

namespace Collector.BL.Services.Feedback
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<DAO.Entities.Feedback> _feedbackRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FeedbackService(IRepository<User> userRepository,
            IHttpContextAccessor httpContextAccessor, IRepository<DAO.Entities.Feedback> feedbackRepository)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _feedbackRepository = feedbackRepository;
        }

        public async Task<FeedbackReturnDTO> AddFeedbackAsync(FeedbackAddDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var creator = await _userRepository.GetByIdAsync(ownerId);
            var newFeedback = model.ToFeedback(creator);
            var addedFeedback = await _feedbackRepository.InsertAsync(newFeedback);
            return addedFeedback.ToFeedbackReturnDTO();
        }

        public async Task AddFeedbackMessageAsync(FeedbackMessageAddDTO model)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var creator = await _userRepository.GetByIdAsync(ownerId);
            var feedback = await _feedbackRepository.GetByIdAsync(model.FeedbackId, feedback1 => feedback1.Messages);
            var newFeedbackMessage = model.ToFeedbackMessage(feedback, creator);
            feedback.Messages.Add(newFeedbackMessage);
            await _feedbackRepository.UpdateAsync(feedback);
        }

        public async Task<FeedbackReturnDTO> GetFeedback(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var feedbackToReturn = await _feedbackRepository.GetFirstAsync(
                feedback => feedback.Creator.Id == ownerId && feedback.Id == id, feedback => feedback.Creator,
                feedback => feedback.Messages, feedback => feedback.ClosedBy);

            return feedbackToReturn.ToFeedbackReturnDTO();
        }

        public async Task CloseFeedback(long id)
        {
            var idClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                throw new UnauthorizedAccessException();
            var ownerId = long.Parse(idClaim.Value);

            var roleClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
            if (roleClaim == null)
                throw new UnauthorizedAccessException();
            Enum.TryParse(roleClaim.Value, out Role ownerRole);


            if (!(ownerRole == Role.Moderator || ownerRole == Role.Admin))
                throw new UnauthorizedAccessException();

            var feedbackToClose =
                await _feedbackRepository.GetFirstAsync(feedback =>
                    feedback.Creator.Id == ownerId && feedback.Id == id);

            var closedBy = await _userRepository.GetByIdAsync(ownerId);
            if (closedBy == null)
                throw new UnauthorizedAccessException();

            feedbackToClose.IsClosed = true;
            feedbackToClose.ClosedBy = closedBy;
            feedbackToClose.Closed = DateTime.UtcNow;

            await _feedbackRepository.UpdateAsync(feedbackToClose);
        }
    }
}