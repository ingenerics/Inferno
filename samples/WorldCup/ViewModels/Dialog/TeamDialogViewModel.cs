using Inferno;
using WorldCup.Domain;
using WorldCup.Repo;
using WorldCup.ViewModels.Overview;

namespace WorldCup.ViewModels.Dialog
{
    public class TeamDialogViewModel : Screen
    {
        public TeamDialogViewModel(TeamOverviewItem teamItem, IRepo<string, Standing> repo)
        {
            TeamItem = teamItem;
            Standing = repo.Get(TeamItem.Team.Code);
        }

        public TeamOverviewItem TeamItem { get; }
        public Standing Standing { get; set; }
    }
}
