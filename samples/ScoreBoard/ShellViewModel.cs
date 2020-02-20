using Inferno;
using ScoreBoard.ViewModels;

namespace ScoreBoard
{
    public class ShellViewModel : ReactiveObject
    {
        public ShellViewModel()
        {
            ScoreBoardViewModel = new ScoreBoardViewModel();
        }

        public ScoreBoardViewModel ScoreBoardViewModel { get; set; }
    }
}
