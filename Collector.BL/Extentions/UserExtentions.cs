using Collector.BL.Models.Authorization;
using Collector.DAO.Entities;
using static System.String;

namespace Collector.BL.Extentions
{
    public static class UserExtentions
    {
        public static UserReturnDTO UserToUserReturnDTO(this User user)
        {
            return new UserReturnDTO
            {
                Email = user.Email,
                Username = user.Username,
                LastName = user.LastName,
                FirstName = user.FirstName,
                UserRole = user.Role.ToString(),
                AvatarUrl = user.AratarUrl ?? "images/defaultAvatar.png"
            };
        }

        public static User RegisterDTOToUser(this RegisterDTO model, long createdBy)
        {
            return new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password.CreateMd5(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = Role.User,
                CreatedBy = createdBy
            };
        }

        public static User UpdateUser(this User user, ChangeProfileDTO model)
        {
            if (model.Email != null)
                user.Email = model.Email;
            if (model.FirstName != null)
                user.FirstName = model.FirstName;
            if (model.LastName != null)
                user.LastName = model.LastName;

            return user;
        }
    }
}