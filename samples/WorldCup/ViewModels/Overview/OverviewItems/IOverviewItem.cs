namespace WorldCup.ViewModels.Overview
{
    public interface IOverviewItem
    {
        int ColumnIndex { get; }
        int RowIndex { get; }
        string HorizontalAlignment { get; }
        string VerticalAlignment { get; }
    }
}