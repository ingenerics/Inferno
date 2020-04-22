using WorldCup.Domain;

namespace WorldCup.ViewModels.Overview
{
    public class MatchOverviewItem : IOverviewItem
    {
        public MatchOverviewItem(Match match, int columnIndex, int rowIndex)
        {
            Match = match;
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
            HorizontalAlignment = "Center";
            VerticalAlignment = "Center";
        }

        public Match Match { get; }
        public string Score => $"{Match.Score1} - {Match.Score2}";
        public int ColumnIndex { get; }
        public int RowIndex { get; }
        public string HorizontalAlignment { get; }
        public string VerticalAlignment { get; }
    }
}