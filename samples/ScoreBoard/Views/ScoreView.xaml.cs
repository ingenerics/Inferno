using Inferno;
using ScoreBoard.ViewModels;
using System.Windows.Media;

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

            this.WhenLoaded(disposables =>
            {
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
            });
        }
    }
}
