using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace WorldCup.Views.Controls
{
    /// <summary>
    /// Interaction logic for GoalItemsControl.xaml
    /// </summary>
    public partial class GoalItemsControl : UserControl
    {
        public GoalItemsControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            ItemsControl.ItemsSourceProperty.AddOwner(typeof(GoalItemsControl));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
    }
}
