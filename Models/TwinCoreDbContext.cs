using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TestTwinCoreProject.Models
{
    public class TwinCoreDbContext : IdentityDbContext<Account>
    {
        public DbSet<FileModel> Files { get; set; }
        public TwinCoreDbContext(DbContextOptions<TwinCoreDbContext> options):base(options)
        {
            Database.EnsureCreated();
        }
    }
}
