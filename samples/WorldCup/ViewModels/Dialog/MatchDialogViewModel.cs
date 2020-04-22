using Inferno;
using WorldCup.ViewModels.Overview;

namespace WorldCup.ViewModels.Dialog
{
    public class MatchDialogViewModel : Screen
    {
        public MatchDialogViewModel(MatchOverviewItem matchItem)
        {
            MatchItem = matchItem;
        }

        public MatchOverviewItem MatchItem { get; }
    }
}
