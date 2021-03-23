using Microsoft.AspNetCore.Identity;
using System;

namespace TestTwinCoreProject.Models
{
    public class Account:IdentityUser<Guid>
    {
        public DateTime DateBirthday { get; set; }
        public DateTime DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
