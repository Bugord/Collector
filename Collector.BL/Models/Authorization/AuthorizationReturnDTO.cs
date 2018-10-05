namespace Collector.BL.Models.Authorization
{
    public class AuthorizationReturnDTO
    {
        public string Token { get; set; }
        public UserReturnDTO User { get; set; }
    }
}
