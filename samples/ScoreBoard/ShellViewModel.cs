using System;
using Inferno;
using ScoreBoard.ViewModels;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ScoreBoard
{
    public class ShellViewModel : Conductor<ScoreBoardViewModel>, IShell
    {
        private readonly IDialogManager _dialogManager;

        public ShellViewModel(IDialogManager dialogManager)
        {
            _dialogManager = dialogManager;

            ActiveItem = new ScoreBoardViewModel();

            this.WhenInitialized(disposables =>
            {
                ActiveItem.CloseCommand
                    .Log("Requested close application")
                    .SelectMany(_ => AskConfirmation())
                    .SubscribeLogger("Confirmed close application")
                    .DisposeWith(disposables);
            });
        }

        private async Task<bool?> AskConfirmation()
        {
            var dialogResult = 
                await _dialogManager.ShowMessageBox(
                    "Confirm", 
                    "Are you sure you want to exit?", 
                    DialogType.Question,
                    ButtonChoice.Yes, ButtonChoice.No);

            if (dialogResult == true)
            {
                RaiseCloseRequest();
            }

            return dialogResult;
        }

        public event Action RequestClose;

        private void RaiseCloseRequest()
        {
            RequestClose?.Invoke();
        }
    }
}
