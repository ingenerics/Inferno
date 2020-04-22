using Inferno;
using WorldCup.ViewModels.Detail;

namespace WorldCup.Views.Detail
{
    /// <summary>
    /// Interaction logic for GroupDetailView.xaml
    /// </summary>
    public partial class GroupDetailView : RxUserControl<GroupDetailViewModel>
    {
        public GroupDetailView()
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
