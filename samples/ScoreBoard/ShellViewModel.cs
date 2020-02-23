using Inferno;
using ScoreBoard.ViewModels;

namespace ScoreBoard
{
    public class ShellViewModel : Conductor<ScoreBoardViewModel>
    {
        public ShellViewModel()
        {
            ActiveItem = new ScoreBoardViewModel();
        }
    }
}
