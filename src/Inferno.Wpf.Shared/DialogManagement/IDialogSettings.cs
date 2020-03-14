using System.Windows;

namespace Inferno
{
    public interface IDialogSettings
    {
        double Width { get; set; }
        double MinWidth { get; set; }
        double Height { get; set; }
        double MinHeight { get; set; }
        WindowStartupLocation WindowStartupLocation { get; set; }
        ResizeMode ResizeMode { get; set; }
    }
}
