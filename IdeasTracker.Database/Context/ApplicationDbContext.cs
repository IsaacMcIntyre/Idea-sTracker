using IdeasTracker.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdeasTracker.Database.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<BackLog> BackLogs { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
