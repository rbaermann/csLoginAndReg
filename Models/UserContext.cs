using Microsoft.EntityFrameworkCore;

namespace loginAndReg.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions options) : base(options) {}

        public DbSet<User> Users {get; set;}
    }
}