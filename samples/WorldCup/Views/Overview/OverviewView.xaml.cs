using Inferno;
using Inferno.Core.Logging;
using WorldCup.ViewModels.Overview;

namespace WorldCup.Views.Overview
{
    /// <summary>
    /// Interaction logic for OverviewView.xaml
    /// </summary>
    public partial class OverviewView : RxUserControl<OverviewViewModel>
    {
        public OverviewView()
        {
            InitializeComponent();

            this.WhenLoaded(disposables =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.Items,
                        v => v.ItemsControl.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(ViewModel,
                        vm => vm.SelectedItem,
                        v => v.ItemsControl.SelectedItem)
                    .DisposeWith(disposables);
            });
        }
    }
}
