using Inferno.Core;

namespace Inferno
{
    /// <summary>
    /// Denotes an instance which implements <see cref="IReactiveObject"/>, <see cref="IHaveDisplayName"/>,
    /// <see cref="IActivate"/> and <see cref="IGuardClose"/>
    /// </summary>
    public interface IScreen : IReactiveObject, IHaveDisplayName, IActivate, IGuardClose
    { 
    }
}
