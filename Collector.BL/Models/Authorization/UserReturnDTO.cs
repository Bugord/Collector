namespace Collector.BL.Models.Authorization
{
    public class UserReturnDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserRole { get; set; }
        public string AvatarUrl { get; set; }
    }
}
