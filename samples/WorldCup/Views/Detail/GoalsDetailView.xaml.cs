using Inferno;
using WorldCup.ViewModels.Detail;

namespace WorldCup.Views.Detail
{
    /// <summary>
    /// Interaction logic for GoalsDetailView.xaml
    /// </summary>
    public partial class GoalsDetailView : RxUserControl<GoalsDetailViewModel>
    {
        public GoalsDetailView()
        {
            InitializeComponent();

            this.WhenLoaded(disposables =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.Items,
                        v => v.ItemsControl.ItemsSource)
                    .DisposeWith(disposables);
            });
        }
    }
}
