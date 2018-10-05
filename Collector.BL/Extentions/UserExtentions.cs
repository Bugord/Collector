using Collector.BL.Models.Authorization;
using Collector.DAO.Entities;

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
                FirstName = user.FirstName
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
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Username = model.Username;

            return user;
        }
    }
}
