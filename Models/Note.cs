using System;
using System.Collections.Generic;

namespace TestTwinCoreProject.Models
{
    public class Note
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public virtual IEnumerable<FileModel> Files { get; set; }
        public DateTime DateTime { get; set; }
    }
}
