using Inferno;
using WorldCup.ViewModels.Dialog;

namespace WorldCup.Views.Dialog
{
    /// <summary>
    /// Interaction logic for GoalsDetailView.xaml
    /// </summary>
    public partial class MatchDialogView : RxUserControl<MatchDialogViewModel>
    {
        public MatchDialogView()
        {
            InitializeComponent();

            this.WhenLoaded(disposables =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.MatchItem.Match.Team1.Name,
                        v => v.NameTeam1.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        vm => vm.MatchItem.Match.Team2.Name,
                        v => v.NameTeam2.Text)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        vm => vm.MatchItem.Match.OutcomeTeam1.Goals,
                        v => v.GoalsTeam1.ItemsSource)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        vm => vm.MatchItem.Match.OutcomeTeam2.Goals,
                        v => v.GoalsTeam2.ItemsSource)
                    .DisposeWith(disposables);
            });
        }
    }
}
