using System.ComponentModel.DataAnnotations;
namespace DataAccess.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string GroupName {  get; set; }
        public string GroupDescription { get; set; }
        public DateTime GroupCreatedDate { get; set; }
        public string TeamMembers { get; set; }
        public IEnumerable<Expense> Expenses { get; set; }

        //public int CreatedByUserId { get; set; }
        [Range(1,10)]
        public int TotalMembers { get; set; }=0;
    }
}
