using System.Reactive.Disposables;
using Inferno;
using ScoreBoard.ViewModels;

namespace ScoreBoard.Views
{
    /// <summary>
    /// Interaction logic for ScoreBoardViewModel.xaml
    /// </summary>
    public partial class ScoreBoardView : RxUserControl<ScoreBoardViewModel>
    {
        public ScoreBoardView()
        {
            InitializeComponent();

            var disposables = new CompositeDisposable();

            this.OneWayBind(ViewModel,
                    viewModel => viewModel.ScoreHomeTeam,
                    view => view.scoreHomeTeam.ViewModel)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    viewModel => viewModel.ScoreVisitors,
                    view => view.scoreVisitors.ViewModel)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                    viewModel => viewModel.NewGameCommand,
                    view => view.NewBtn)
                .DisposeWith(disposables);
        }
    }
}
