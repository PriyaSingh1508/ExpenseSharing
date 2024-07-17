

namespace DataAccess.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public decimal CurrentBalance { get; set; } = 0;
        public string Roles { get; set; }

        public string GroupsName { get; set; }

    }
}
