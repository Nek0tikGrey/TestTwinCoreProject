using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTwinCoreProject.Models
{
    public class Note
    {
        public Guid Id { get; set; }
        public string AccountId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public virtual IEnumerable<FileModel> Files { get; set; }
        public DateTime DateTime { get; set; }
    }
}
