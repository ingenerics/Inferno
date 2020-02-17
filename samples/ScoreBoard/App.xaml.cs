using ScoreBoard.Bootstrap;
using System.Windows;

namespace ScoreBoard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            new AppBootstrapper(this);
        }
    }
}
