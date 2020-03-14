namespace Inferno
{
    /// <summary>
    /// Denotes an object that can set the Dialog result of any bound view.
    /// </summary>
    public interface IHaveDialogResult
    {
        bool? DialogResult { get; set; }
    }
}
