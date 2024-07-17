

namespace Shared.DTOs
{
    public class GroupInfoDto
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public DateTime GroupCreatedDate { get; set; }
        public string TeamMembers { get; set; }
        public int TotalMembers { get; set; } = 0;
    }
}
