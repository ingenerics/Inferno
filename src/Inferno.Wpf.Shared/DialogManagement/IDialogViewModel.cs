using Inferno.Core;
using System.Windows.Input;

namespace Inferno
{
    /// <summary>
    /// The contract for a view model to be shown with the <see cref="DialogManager"/>.
    /// </summary>
    public interface IDialogViewModel<TChoice> : IHaveDialogResult, IHaveDisplayName
    {
        DialogType DialogType { get; }
        IScreen ViewModel { get; }
        IBindableCollection<ButtonContext<TChoice>> Buttons { get; }
        ICommand ButtonClickCommand { get; }
    }
}
