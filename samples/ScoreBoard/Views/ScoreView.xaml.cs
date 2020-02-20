using System.Reactive.Disposables;
using System.Windows.Media;
using Inferno;
using ScoreBoard.ViewModels;

namespace ScoreBoard.Views
{
    /// <summary>
    /// Interaction logic for ScoreView.xaml
    /// </summary>
    public partial class ScoreView : RxUserControl<ScoreViewModel>
    {
        public ScoreView()
        {
            InitializeComponent();

            var disposables = new CompositeDisposable();

            this.OneWayBind(ViewModel,
                    viewModel => viewModel.BackgroundColor,
                    view => view.Panel.Background,
                    hex => new BrushConverter().ConvertFrom(hex))
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    viewModel => viewModel.Score,
                    view => view.ScoreLabel.Content)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                    viewModel => viewModel.DecrementScoreCommand,
                    view => view.DecrBtn)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                    viewModel => viewModel.IncrementScoreCommand,
                    view => view.IncrBtn)
                .DisposeWith(disposables);
        }
    }
}
