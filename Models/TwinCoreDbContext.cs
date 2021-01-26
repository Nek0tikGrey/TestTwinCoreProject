using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace TestTwinCoreProject.Models
{
    public class TwinCoreDbContext : IdentityDbContext<Account,ApplicationRole,Guid>
    {
        public DbSet<FileModel> Files { get; set; }
        public TwinCoreDbContext(DbContextOptions<TwinCoreDbContext> options):base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Account>().Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
