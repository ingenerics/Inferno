using System.Windows;

namespace Inferno
{
    public class DialogSettings : IDialogSettings
    {
        public DialogSettings()
        {
            Width = MinWidth = Height = MinHeight = Constants.Default;
        }

        public double Width { get; set; }
        public double MinWidth { get; set; }
        public double Height { get; set; }
        public double MinHeight { get; set; }
        public WindowStartupLocation WindowStartupLocation { get; set; }
        public ResizeMode ResizeMode { get; set; }
    }
}
