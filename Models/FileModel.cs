using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTwinCoreProject.Models
{
    public class FileModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }
        public string Extensions { get; set; }
        public Type TypeTo { get; set; }
        public Guid Guid { get; set; }

        public enum Type
        {
            Avatar,
            Note
        }
    }

}
