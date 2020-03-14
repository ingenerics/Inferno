using Inferno.Core;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Windows.Input;

namespace Inferno
{
    public class DialogViewModel<TChoice> : Conductor<IScreen>, IDialogViewModel<TChoice>
    {
        private readonly Action<ButtonContext<TChoice>> _buttonHandler;

        public DialogViewModel(string title, IScreen viewModel, IEnumerable<ButtonContext<TChoice>> buttons, DialogType dialogType = DialogType.None, Action<ButtonContext<TChoice>> buttonHandler = null)
        {
            if (viewModel == null || buttons == null) throw new ArgumentNullException();

            DisplayName = title;
            DialogType = dialogType;

            ViewModel = viewModel;
            Buttons = new BindableCollection<ButtonContext<TChoice>>(buttons);
            _buttonHandler = buttonHandler;

            this.WhenInitialized(disposables =>
            {
                ButtonClickCommand = ReactiveCommand.Create<ButtonContext<TChoice>, Unit>(OnButtonClicked).DisposeWith(disposables);
            });
        }

        protected virtual Unit OnButtonClicked(ButtonContext<TChoice> button)
        {
            _buttonHandler?.Invoke(button);

            DialogResult = button.IsDefault;

            return Unit.Default;
        }

        public DialogType DialogType { get; }
        public IScreen ViewModel { get; }
        public IBindableCollection<ButtonContext<TChoice>> Buttons { get; }
        public ICommand ButtonClickCommand { get; private set; }


        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            set => this.RaiseAndSetIfChanged(ref _dialogResult, value);
        }
    }
}
