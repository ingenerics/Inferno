using Inferno;
using System.Reactive;
using System.Reactive.Linq;

namespace ScoreBoard.ViewModels
{
    public class ScoreBoardViewModel : ReactiveObject
    {
        private readonly CombinedReactiveCommand<Unit, int> ResetScoresCommand;

        public ScoreBoardViewModel()
        {
            ScoreHomeTeam = new ScoreViewModel("#FF024D70");
            ScoreVisitors = new ScoreViewModel("#FF7E0E03");

            ResetScoresCommand = ReactiveCommand.CreateCombined(
                new ReactiveCommand<Unit, int>[]
                {
                    ScoreHomeTeam.ResetScoreCommand,
                    ScoreVisitors.ResetScoreCommand
                });

            var canStartNewGame =
                Observable.CombineLatest(
                    ScoreHomeTeam.CanDecrement,
                    ScoreVisitors.CanDecrement,
                    (canHomeDecr, canVisitorsDecr) => canHomeDecr || canVisitorsDecr);
            NewGameCommand = ReactiveCommand.Create(() => Unit.Default, canStartNewGame);
            NewGameCommand.InvokeCommand(ResetScoresCommand);
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

        public ReactiveCommand<Unit, Unit> NewGameCommand { get; }
    }
}
