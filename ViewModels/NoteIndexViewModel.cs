using System.Collections.Generic;
using TestTwinCoreProject.Models;

namespace TestTwinCoreProject.ViewModels
{
    public class NoteIndexViewModel
    {
        public IEnumerable<Note> Notes { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
    }
}
