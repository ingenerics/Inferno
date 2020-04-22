using Inferno;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScoreBoard.ViewModels
{
    public class ScoreBoardViewModel : Conductor<ScoreViewModel>.Collection.AllActive
    {
        public ScoreBoardViewModel()
        {
            this.WhenInitialized(disposables =>
            {
                var canStartNewGame =
                    Observable.CombineLatest(
                        ScoreHomeTeam.CanDecrement,
                        ScoreVisitors.CanDecrement,
                        (canHomeDecr, canVisitorsDecr) => canHomeDecr || canVisitorsDecr);

                NewGameCommand = 
                    ReactiveCommand.CreateCombined(
                        new[]
                        {
                            ScoreHomeTeam.ResetScoreCommand,
                            ScoreVisitors.ResetScoreCommand
                        },
                        canStartNewGame)
                    .DisposeWith(disposables);
                
                CloseCommand = ReactiveCommand.Create(() => Unit.Default).DisposeWith(disposables);
            });
        }

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            ScoreHomeTeam = new ScoreViewModel("#FF024D70") { DisplayName = nameof(ScoreHomeTeam) };
            ScoreVisitors = new ScoreViewModel("#FF7E0E03") { DisplayName = nameof(ScoreVisitors) };

            Items.Add(ScoreHomeTeam);
            Items.Add(ScoreVisitors);

            return Task.CompletedTask;
        }

        private ScoreViewModel _scoreHomeTeam;
        public ScoreViewModel ScoreHomeTeam
        {
            get => _scoreHomeTeam;
            set => this.RaiseAndSetIfChanged(ref _scoreHomeTeam, value);
        }

        private ScoreViewModel _scoreVisitors;
        public ScoreViewModel ScoreVisitors
        {
            get => _scoreVisitors;
            set => this.RaiseAndSetIfChanged(ref _scoreVisitors, value);
        }

        public CombinedReactiveCommand<Unit, int> NewGameCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; private set; }
    }
}
