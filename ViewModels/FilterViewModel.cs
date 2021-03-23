using System;

namespace TestTwinCoreProject.ViewModels
{
    public class FilterViewModel
    {
        public string SelectedNoteTheme { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public FilterViewModel(string theme,DateTime dateFrom,DateTime dateTo)
        {
            DateFrom = dateFrom;
            DateTo = dateTo;
        }
    }
}
