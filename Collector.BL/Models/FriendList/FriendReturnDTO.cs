namespace Collector.BL.Models.FriendList
{
    public class FriendReturnDTO
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public bool IsSynchronized { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
