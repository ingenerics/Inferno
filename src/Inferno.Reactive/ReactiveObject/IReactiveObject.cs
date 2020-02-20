using System.ComponentModel;

namespace Inferno
{
    /// <summary>
    /// A reactive object is a interface for ViewModels which will notify when properties are either changing or changed.
    /// The primary use of this interface is to allow external classes, such as the RxPropertyHelper, 
    /// to trigger these events inside the ViewModel.
    /// </summary>
    public interface IReactiveObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// Raise a property is changing event.
        /// </summary>
        /// <param name="args">The arguments with details about the property that is changing.</param>
        void RaisePropertyChanging(PropertyChangingEventArgs args);

        /// <summary>
        /// Raise a property has changed event.
        /// </summary>
        /// <param name="args">The arguments with details about the property that has changed.</param>
        void RaisePropertyChanged(PropertyChangedEventArgs args);
    }
}