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
        }
    }
}
