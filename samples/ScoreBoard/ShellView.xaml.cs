﻿using Inferno;

namespace ScoreBoard
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : RxWindow<ShellViewModel>
    {
        public ShellView()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel,
                viewModel => viewModel.ActiveItem,
                view => view.Host.ViewModel);
        }
    }
}
