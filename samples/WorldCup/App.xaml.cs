using System.Windows;
using WorldCup.Bootstrap;

namespace WorldCup
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
