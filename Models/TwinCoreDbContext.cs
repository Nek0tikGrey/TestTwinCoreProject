using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestTwinCoreProject.Models
{
    public class TwinCoreDbContext : IdentityDbContext<Account,ApplicationRole,Guid>
    {
        public DbSet<FileModel> Files { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<InviteUser> InviteUsers { get; set; }

        public TwinCoreDbContext(DbContextOptions<TwinCoreDbContext> options):base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Account>().Property(p => p.Id).ValueGeneratedOnAdd();
        }

        private void DbSaveChanges()
        {
            var date = DateTime.Now;
            var Entities = ChangeTracker.Entries().Where(prop => prop.State == EntityState.Deleted);
            foreach(var entity in Entities)
            {
                if(entity.Entity is not Account)
                {
                    continue;
                }
                if (!(bool)entity.CurrentValues["IsDeleted"])
                {
                    entity.State = EntityState.Modified;
                    entity.CurrentValues["IsDeleted"] = true;
                    entity.CurrentValues["DeletedDate"] = date;
                }
            }
        }

        public override int SaveChanges()
        {
            DbSaveChanges();
            return base.SaveChanges();
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DbSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            DbSaveChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            DbSaveChanges();
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
