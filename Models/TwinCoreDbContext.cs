using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTwinCoreProject.Models
{
    public class TwinCoreDbContext:IdentityDbContext<Account>
    {

        public TwinCoreDbContext(DbContextOptions<TwinCoreDbContext> options):base(options)
        {
            Database.EnsureCreated();
        }
    }
}
