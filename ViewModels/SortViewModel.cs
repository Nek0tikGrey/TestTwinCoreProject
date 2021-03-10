namespace TestTwinCoreProject.ViewModels
{
    public class SortViewModel
    {
        public SortState ThemeSort { get; private set; } 
        public SortState DateSort { get; private set; }    
        public SortState Current { get; private set; }

        public SortViewModel(SortState sortOrder)
        {
            ThemeSort = sortOrder == SortState.ThemeAsc ? SortState.ThemeDesc : SortState.ThemeAsc;
            DateSort = sortOrder == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;
            Current = sortOrder;
        }

    }
    public enum SortState
    {
        ThemeAsc, 
        ThemeDesc,  
        DateAsc,
        DateDesc
    }
}
