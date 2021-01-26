using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestTwinCoreProject.Models
{
    public class Account:IdentityUser<Guid>
    {
        public DateTime DateBirthday { get; set; }
    }
}
