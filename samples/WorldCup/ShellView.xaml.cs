using Inferno;
using System;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WorldCup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellView : RxWindow<ShellViewModel>
    {
        public ShellView()
        {
            InitializeComponent();

            this.WhenLoaded(disposables =>
            {
                #region Title bar

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Icon,
                        view => view.Icon,
                        uri => new BitmapImage(uri))
                    .DisposeWith(disposables);

                #endregion Title bar

                #region Shell

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Years,
                    view => view.CupYear.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(ViewModel,
                    viewModel => viewModel.SelectedYear,
                    view => view.CupYear.SelectedItem)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.Logger.ItemsSource,
                    view => view.Logs.ItemsSource)
                    .DisposeWith(disposables);

                this.ScrollViewer.Events().ScrollChanged
                    .Where(arg => Math.Abs(arg.ExtentHeightChange) > 0.001)
                    .Do(_ => ScrollViewer.ScrollToEnd())
                    .Subscribe()
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.ExpandItemCommand,
                        view => view.ExpandItemBtn)
                    .DisposeWith(disposables);

                #endregion Shell

                #region Hosts

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.OverviewViewModel,
                    view => view.OverviewHost.ViewModel)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.DetailViewModel.ActiveItem,
                    view => view.DetailHost.ViewModel)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    viewModel => viewModel.DetailViewModel.DetailOptions,
                    view => view.Detail.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(ViewModel,
                    viewModel => viewModel.DetailViewModel.SelectedDetail,
                    view => view.Detail.SelectedItem)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.DetailViewModel.CleanUpCommand,
                        view => view.CleanUpDetailBtn)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.DetailViewModel.RefreshCommand,
                        view => view.RefreshDetailBtn)
                    .DisposeWith(disposables);

                #endregion Hosts
            });
        }
    }
}
