using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime BirthDate { get; set; }
        public IList<FileModel> Avatars { get; set; }

        public UserAccountViewModel()
        {
            Avatars = new List<FileModel>();
        }
    }
}
