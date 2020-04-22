using WorldCup.Domain;

namespace WorldCup.ViewModels.Overview
{
    public class TeamOverviewItem : IOverviewItem
    {
        public TeamOverviewItem(Team team, int columnIndex, int rowIndex)
        {
            Team = team;
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
            HorizontalAlignment = "Center";
            VerticalAlignment = "Center";
        }

        public Team Team { get; }
        public int ColumnIndex { get; }
        public int RowIndex { get; }
        public string HorizontalAlignment { get; }
        public string VerticalAlignment { get; }
    }
}