namespace WorldCup.ViewModels.Overview
{
    public class LineOverviewItem : IOverviewItem
    {
        public LineOverviewItem(int columnIndex, int rowIndex)
        {
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
            HorizontalAlignment = "Stretch";
            VerticalAlignment = "Center";
        }

        public int ColumnIndex { get; }
        public int RowIndex { get; }
        public string HorizontalAlignment { get; }
        public string VerticalAlignment { get; }
    }
}