using Inferno;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ScoreBoard.ViewModels
{
    public class ScoreViewModel : Screen
    {
        private RxPropertyHelper<int> _score;

        public ScoreViewModel(string backgroundColor)
        {
            BackgroundColor = backgroundColor;

            this.WhenInitialized(disposables =>
            {
                IncrementScoreCommand = ReactiveCommand.Create(() => 1).DisposeWith(disposables);

                CanDecrement =
                    this.WhenInitializedSwitch(
                        this
                            .WhenAnyValue(x => x.Score)
                            .Select(score => score > 0),
                        false);

                DecrementScoreCommand = ReactiveCommand.Create(() => -1, CanDecrement).DisposeWith(disposables);

                ResetScoreCommand = ReactiveCommand.Create(() => -Score).DisposeWith(disposables);

                _score =
                    Observable.Merge(
                        IncrementScoreCommand,
                        DecrementScoreCommand,
                        ResetScoreCommand)
                    .Scan(0, (acc, delta) => acc + delta)
                    .ToProperty(this, x => x.Score)
                    .DisposeWith(disposables);
            });
        }

        public int Score => _score.Value;

        public IObservable<bool> CanDecrement { get; private set; }
        public ReactiveCommand<Unit, int> IncrementScoreCommand { get; private set; }
        public ReactiveCommand<Unit, int> DecrementScoreCommand { get; private set; }
        public ReactiveCommand<Unit, int> ResetScoreCommand { get; private set; }

        public string BackgroundColor { get; }
    }
}
