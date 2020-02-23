using Inferno;
using System.Reactive;
using System.Reactive.Linq;

namespace ScoreBoard.ViewModels
{
    public class ScoreBoardViewModel : Conductor<ScoreViewModel>.Collection.AllActive
    {
        public ScoreBoardViewModel()
        {
            ScoreHomeTeam = new ScoreViewModel("#FF024D70") { DisplayName = nameof(ScoreHomeTeam) };
            ScoreVisitors = new ScoreViewModel("#FF7E0E03") { DisplayName = nameof(ScoreVisitors) };

            Items.Add(ScoreHomeTeam);
            Items.Add(ScoreVisitors);

            this.WhenActivated(disposables =>
            {
                var resetScoresCommand = ReactiveCommand.CreateCombined(
                    new[]
                    {
                        ScoreHomeTeam.ResetScoreCommand,
                        ScoreVisitors.ResetScoreCommand
                    }).DisposeWith(disposables);

                var canStartNewGame =
                    Observable.CombineLatest(
                        ScoreHomeTeam.CanDecrement,
                        ScoreVisitors.CanDecrement,
                        (canHomeDecr, canVisitorsDecr) => canHomeDecr || canVisitorsDecr);
                NewGameCommand = ReactiveCommand.Create(() => Unit.Default, canStartNewGame).DisposeWith(disposables);
                NewGameCommand.InvokeCommand(resetScoresCommand);
            });
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

        public ReactiveCommand<Unit, Unit> NewGameCommand { get; private set; }
    }
}
