using System.Windows.Controls;
using Inferno.Core;

namespace Inferno.Reactive.Tests
{
    public class CommandBindingView : IViewFor<CommandBindingViewModel>
    {
        public CommandBindingView()
        {
            Command1 = new CustomClickButton();
            Command2 = new Image();
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CommandBindingViewModel)value;
        }

        public CommandBindingViewModel ViewModel { get; set; }

        public CustomClickButton Command1 { get; protected set; }

        public Image Command2 { get; protected set; }
    }
}
