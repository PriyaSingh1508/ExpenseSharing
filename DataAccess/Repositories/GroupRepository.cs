using DataAccess.Data;
using DataAccess.Entities;
using Shared.DTOs;


namespace DataAccess.Repositories
{
    public class GroupRepository:IGroupRepository
    {
        private readonly ExpenseDbContext _context;

        public GroupRepository(ExpenseDbContext context)
        {
            _context = context;
        }
        public async Task<string> CreateGroup(GroupDTO group) // Unit-Test Class
        {
            if (group.TotalMembers > 10) return "A group cannot have more than 10 members";
            var createGroup = new Group()
            {
                GroupName = group.GroupName,
                GroupDescription = group.GroupDescription,
                GroupCreatedDate = DateTime.Now,
                TeamMembers = group.TeamMembers,
               TotalMembers = group.TotalMembers,
            };

            await _context.AddAsync(createGroup);
            await _context.SaveChangesAsync();
            return createGroup.Id.ToString(); 
        }
        public bool DeleteGroup(int id)
        {
            var existingGroup = _context.Groups.FirstOrDefault(g => g.Id == id);
            if (existingGroup != null)
            {
                _context.Groups.Remove(existingGroup);
                return true;
            }
            return false;
        }


        public Group FindById(int Id)
        {
            var existingGroup =  _context.Groups.FirstOrDefault(x=> x.Id==Id);
                return existingGroup;
        }

    }
}
