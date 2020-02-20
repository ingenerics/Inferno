using Inferno.Core;

namespace Inferno
{
    /// <summary>
    /// Denotes an instance which implements <see cref="IHaveDisplayName"/>, <see cref="IActivate"/>, 
    /// <see cref="IGuardClose"/> and <see cref="IReactiveObject"/>
    /// </summary>
    public interface IScreen : IHaveDisplayName, IActivate, IGuardClose, IReactiveObject
    {
    }
}
