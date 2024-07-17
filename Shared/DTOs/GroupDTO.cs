using System.Text.Json.Serialization;

namespace Shared.DTOs
{
    public class GroupDTO
    {
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public string TeamMembers { get; set; }
        [JsonIgnore]
        public int TotalMembers { get; set; } = 0;
    }
}
