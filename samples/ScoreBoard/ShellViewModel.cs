using Inferno;
using ScoreBoard.ViewModels;
using System.Reactive.Linq;

namespace ScoreBoard
{
    public class ShellViewModel : Conductor<ScoreBoardViewModel>, IShell
    {
        public ShellViewModel()
        {
            ActiveItem = new ScoreBoardViewModel();

            this.WhenInitialized(disposables =>
            {
                this.ActiveItem.CloseCommand
                    .Select(_ => RequestClose = true)
                    .SubscribeLogger("Requested close application")
                    .DisposeWith(disposables);
            });
        }

        private bool _requestClose;
        public bool RequestClose
        {
            get => _requestClose;
            set
            {
                this.RaiseAndSetIfChanged(ref _requestClose, value);
                // Reset, so we can re-initiate close after user has cancelled.
                this.RaiseAndSetIfChanged(ref _requestClose, false);
            }
        } 
    }
}
