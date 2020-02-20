using Inferno;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ScoreBoard.ViewModels
{
    public class ScoreViewModel : ReactiveObject
    {
        private readonly RxPropertyHelper<int> _score;

        public ScoreViewModel(string backgroundColor)
        {
            BackgroundColor = backgroundColor;

            IncrementScoreCommand = ReactiveCommand.Create(() => 1);

            CanDecrement =
                this
                .WhenAnyValue(x => x.Score)
                .Select(score => score > 0);

            DecrementScoreCommand = ReactiveCommand.Create(() => -1, CanDecrement);

            ResetScoreCommand = ReactiveCommand.Create(() => -Score);

            _score = 
                Observable.Merge(
                    IncrementScoreCommand,
                    DecrementScoreCommand,
                    ResetScoreCommand)
                .Scan(0, (acc, delta) => acc + delta)
                .ToProperty(this, x => x.Score);
        }

        public int Score => _score?.Value ?? 0;

        public readonly IObservable<bool> CanDecrement;
        public ReactiveCommand<Unit, int> IncrementScoreCommand { get; }
        public ReactiveCommand<Unit, int> DecrementScoreCommand { get; }
        public ReactiveCommand<Unit, int> ResetScoreCommand { get; }

        public string BackgroundColor { get; }
    }
}
