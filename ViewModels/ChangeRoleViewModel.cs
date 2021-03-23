using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTwinCoreProject.Models;

namespace TestTwinCoreProject.ViewModels
{
    public class ChangeRoleViewModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public List<ApplicationRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }

        public ChangeRoleViewModel()
        {
            AllRoles = new List<ApplicationRole>();
            UserRoles = new List<string>();
        }
    }
}
