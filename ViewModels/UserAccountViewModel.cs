using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTwinCoreProject.Models;

namespace TestTwinCoreProject.ViewModels
{
    public class UserAccountViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public IList<FileModel> Avatars { get; set; }

        public UserAccountViewModel()
        {
            Avatars = new List<FileModel>();
        }
    }
}
