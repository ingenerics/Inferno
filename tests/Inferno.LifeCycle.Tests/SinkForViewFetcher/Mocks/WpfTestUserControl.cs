using Inferno.Core;
using System.Windows.Controls;

namespace Inferno.LifeCycle.Tests
{
    public class WpfTestUserControl : UserControl, IViewFor
    {
        public object ViewModel { get; set; }
    }
}
