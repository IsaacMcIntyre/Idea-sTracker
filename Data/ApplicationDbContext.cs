using IdeasTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace IdeasTracker.Data
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