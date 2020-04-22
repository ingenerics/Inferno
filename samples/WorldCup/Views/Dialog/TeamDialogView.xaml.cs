using Inferno;
using WorldCup.ViewModels.Dialog;

namespace WorldCup.Views.Dialog
{
    /// <summary>
    /// Interaction logic for GoalsDetailView.xaml
    /// </summary>
    public partial class TeamDialogView : RxUserControl<TeamDialogViewModel>
    {
        public TeamDialogView()
        {
            InitializeComponent();

            this.WhenLoaded(disposables =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.TeamItem.Team.Name,
                        v => v.TeamName.Text)
                    .DisposeWith(disposables);

                #region Matches

                this.OneWayBind(ViewModel,
                        vm => vm.Standing.Played,
                        v => v.MatchesPlayed.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        vm => vm.Standing.Won,
                        v => v.MatchesWon.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        vm => vm.Standing.Drawn,
                        v => v.MatchesTied.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        vm => vm.Standing.Lost,
                        v => v.MatchesLost.Text)
                    .DisposeWith(disposables);

                #endregion Matches

                #region Goals

                this.OneWayBind(ViewModel,
                        vm => vm.Standing.Goals_For,
                        v => v.GoalsScored.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        vm => vm.Standing.Goals_Against,
                        v => v.GoalsAgainst.Text)
                    .DisposeWith(disposables);

                #endregion Goals
            });
        }
    }
}
