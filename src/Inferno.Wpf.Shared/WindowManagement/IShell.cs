using System;

namespace Inferno
{
    /// <summary>
    /// Denotes an object that can request any bound view to close.
    /// </summary>
    public interface IShell
    {
        event Action RequestClose;
    }
}
