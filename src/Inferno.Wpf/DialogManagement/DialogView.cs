using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Inferno.DialogManagement.Views
{
    public class DialogView<TChoice> : RxWindow<IDialogViewModel<TChoice>>
    {
        public DialogView()
        {
            Buttons = CreateButtons();
            Host = new CompositionControl();
            Content = CreateDialogContent(Buttons, Host);

            this.WhenLoaded(disposables =>
            {
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.DisplayName,
                        view => view.Title)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Buttons,
                        view => view.Buttons.ItemsSource)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.ViewModel,
                        view => view.Host.ViewModel)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.ButtonClickCommand,
                        view => view.ButtonClickCommand)
                    .DisposeWith(disposables);
            });
        }

        private static ItemsControl CreateButtons()
        {
            var itemsControl = new ItemsControl
            {
                ItemsPanel = GetItemsPanelTemplate(),
                ItemTemplate = GetItemTemplate(),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            itemsControl.SetValue(DockPanel.DockProperty, Dock.Bottom);

            return itemsControl;
        }

        private static ItemsPanelTemplate GetItemsPanelTemplate()
        {
            FrameworkElementFactory itemsPanel = new FrameworkElementFactory(typeof(StackPanel));

            itemsPanel.SetValue(Panel.IsItemsHostProperty, true);
            itemsPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            ItemsPanelTemplate template = new ItemsPanelTemplate();

            template.VisualTree = itemsPanel;
            template.Seal();
            return template;
        }

        private static DataTemplate GetItemTemplate()
        {
            DataTemplate template = new DataTemplate();

            FrameworkElementFactory button = new FrameworkElementFactory(typeof(Button));
            button.SetBinding(Button.ContentProperty, new Binding(nameof(ButtonContext<ButtonChoice>.Choice)));
            button.SetBinding(Button.IsCancelProperty, new Binding(nameof(ButtonContext<ButtonChoice>.IsCancel)));
            button.SetBinding(Button.IsDefaultProperty, new Binding(nameof(ButtonContext<ButtonChoice>.IsDefault)));
            button.SetBinding(Button.CommandProperty, new Binding(nameof(ButtonClickCommand)) { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor) { AncestorLevel = 1, AncestorType = typeof(DialogView<TChoice>) } });
            button.SetBinding(Button.CommandParameterProperty, new Binding());
            button.SetValue(Button.WidthProperty, 75.0);
            button.SetValue(Button.MarginProperty, new Thickness(10, 10, 10, 10));

            template.VisualTree = button;
            template.Seal();
            return template;
        }

        private static object CreateDialogContent(ItemsControl buttons, CompositionControl host)
        {
            var dock = new DockPanel
            {
                Focusable = false,
                LastChildFill = true,
                Margin = new Thickness(20, 20, 20, 20)
            };

            dock.Children.Add(buttons);
            dock.Children.Add(host);

            return dock;
        }

        public ItemsControl Buttons
        {
            get { return (ItemsControl)GetValue(ButtonsProperty); }
            set { SetValue(ButtonsProperty, value); }
        }

        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register(nameof(Buttons), typeof(ItemsControl), typeof(DialogView<TChoice>), new PropertyMetadata(null));

        public CompositionControl Host
        {
            get { return (CompositionControl)GetValue(HostProperty); }
            set { SetValue(HostProperty, value); }
        }

        public static readonly DependencyProperty HostProperty =
            DependencyProperty.Register(nameof(Host), typeof(CompositionControl), typeof(DialogView<TChoice>), new PropertyMetadata(null));

        public ICommand ButtonClickCommand
        {
            get { return (ICommand)GetValue(ButtonClickCommandProperty); }
            set { SetValue(ButtonClickCommandProperty, value); }
        }

        public static readonly DependencyProperty ButtonClickCommandProperty =
            DependencyProperty.Register(nameof(ButtonClickCommand), typeof(ICommand), typeof(DialogView<TChoice>), new PropertyMetadata(null));
    }
}